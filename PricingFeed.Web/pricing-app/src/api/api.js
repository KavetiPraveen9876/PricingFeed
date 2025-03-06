import axios from "axios";

const API_BASE_URL = "http://localhost:5000/api"; // Replace with your actual API URL

// Get stored token from localStorage
const getAuthToken = () => {
  return localStorage.getItem("token");
};

// Create an Axios instance with authorization headers
const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    "Content-Type": "application/json",
  },
});

// Add a request interceptor to attach the token
apiClient.interceptors.request.use(
  (config) => {
    const token = getAuthToken();
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// 🔹 **Auth API**
export const loginUser = async (credentials) => {
  const response = await apiClient.post("/auth/login", credentials);
  return response.data;
};

export const registerUser = async (userData) => {
  const response = await apiClient.post("/auth/register", userData);
  return response.data;
};

// 🔹 **CSV Upload API**
export const uploadCsv = async (formData) => {
  const response = await apiClient.post("/pricing/upload", formData, {
    headers: { "Content-Type": "multipart/form-data" },
  });
  return response.data;
};

// 🔹 **Search Pricing Data API**
export const searchPricingData = async (queryParams) => {
  const response = await apiClient.get("/pricing/search", { params: queryParams });
  return response.data;
};

// 🔹 **Edit Pricing Data API**
export const updatePricingRecord = async (id, updatedData) => {
  const response = await apiClient.put(`/pricing/edit/${id}`, updatedData);
  return response.data;
};

// 🔹 **Fetch Pricing Record by ID**
export const getPricingById = async (id) => {
  const response = await apiClient.get(`/pricing/${id}`);
  return response.data;
};

// 🔹 **Logout (Clear Token)**
export const logoutUser = () => {
  localStorage.removeItem("token");
};

export default apiClient;
