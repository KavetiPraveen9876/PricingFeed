using Amazon.CDK;
using Amazon.CDK.AWS.S3;
using Amazon.CDK.AWS.RDS;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.ECS;
using Amazon.CDK.AWS.ECS.Patterns;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Cognito;
using Amazon.CDK.AWS.SQS;
using Amazon.CDK.AWS.Lambda.EventSources;
using Constructs;

namespace PricingAppCDK
{
    public class PricingAppStack : Stack
    {
        public PricingAppStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            
            var s3Bucket = new Bucket(this, "PricingCsvBucket", new BucketProps
            {
                RemovalPolicy = RemovalPolicy.DESTROY,
                AutoDeleteObjects = true
            });

        
            var csvProcessingQueue = new Queue(this, "CsvProcessingQueue", new QueueProps
            {
                VisibilityTimeout = Duration.Seconds(300), // Ensures Lambda has enough time
                RetentionPeriod = Duration.Days(4) // Retains failed messages for 4 days
            });

          
            var vpc = new Amazon.CDK.AWS.EC2.Vpc(this, "PricingVpc");

            var dbInstance = new DatabaseInstance(this, "PricingRDS", new DatabaseInstanceProps
            {
                Engine = DatabaseInstanceEngine.SqlServerSe(new SqlServerSeInstanceEngineProps { Version = SqlServerEngineVersion.VER_15 }),
                InstanceType = Amazon.CDK.AWS.EC2.InstanceType.Of(Amazon.CDK.AWS.EC2.InstanceClass.BURSTABLE3, Amazon.CDK.AWS.EC2.InstanceSize.MICRO),
                Vpc = vpc,
                MultiAz = false,
                Credentials = Credentials.FromGeneratedSecret("admin"),
                RemovalPolicy = RemovalPolicy.DESTROY
            });

        
            var lambdaRole = new Role(this, "LambdaExecutionRole", new RoleProps
            {
                AssumedBy = new ServicePrincipal("lambda.amazonaws.com"),
                ManagedPolicies = new[]
                {
                    ManagedPolicy.FromAwsManagedPolicyName("service-role/AWSLambdaBasicExecutionRole")
                }
            });

            var csvProcessingLambda = new Function(this, "CsvProcessingLambda", new FunctionProps
            {
                Runtime = Runtime.DOTNET_6,
                Handler = "CsvProcessor::CsvProcessor.LambdaHandler::Handler",
                Code = Code.FromAsset("lambda"),
                Role = lambdaRole
            });

            csvProcessingLambda.AddEventSource(new SqsEventSource(csvProcessingQueue));

      
            var cluster = new Cluster(this, "PricingCluster", new ClusterProps { Vpc = vpc });

            var fargateService = new ApplicationLoadBalancedFargateService(this, "PricingService", new ApplicationLoadBalancedFargateServiceProps
            {
                Cluster = cluster,
                TaskImageOptions = new ApplicationLoadBalancedTaskImageOptions
                {
                    Image = ContainerImage.FromRegistry("your-dockerhub-account/pricing-api"), // Replace with actual image
                    Environment = new Dictionary<string, string> {
                        { "DB_HOST", dbInstance.DbInstanceEndpointAddress }
                    }
                },
                MemoryLimitMiB = 512,
                DesiredCount = 2
            });

            dbInstance.Connections.AllowDefaultPortFrom(fargateService.Service);

         
            var api = new RestApi(this, "PricingApi", new RestApiProps
            {
                RestApiName = "Pricing Service API",
                Description = "API Gateway for Pricing Management"
            });

            var pricingResource = api.Root.AddResource("pricing");
            pricingResource.AddMethod("GET", new LambdaIntegration(csvProcessingLambda)); // Connect to Lambda

        
            var userPool = new UserPool(this, "PricingUserPool", new UserPoolProps
            {
                UserPoolName = "PricingUsers",
                SelfSignUpEnabled = true,
                SignInAliases = new SignInAliases { Email = true }
            });

            var userPoolClient = new UserPoolClient(this, "PricingUserPoolClient", new UserPoolClientProps
            {
                UserPool = userPool
            });

           
            s3Bucket.GrantReadWrite(csvProcessingLambda);
            csvProcessingQueue.GrantConsumeMessages(csvProcessingLambda);

          
            new CfnOutput(this, "S3BucketName", new CfnOutputProps { Value = s3Bucket.BucketName });
            new CfnOutput(this, "SQSQueueUrl", new CfnOutputProps { Value = csvProcessingQueue.QueueUrl });
            new CfnOutput(this, "RDSInstanceEndpoint", new CfnOutputProps { Value = dbInstance.DbInstanceEndpointAddress });
            new CfnOutput(this, "ApiGatewayUrl", new CfnOutputProps { Value = api.Url });
            new CfnOutput(this, "CognitoUserPoolId", new CfnOutputProps { Value = userPool.UserPoolId });
        }
    }
}
