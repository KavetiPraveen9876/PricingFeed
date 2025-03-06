import { useState } from "react";
import axios from "axios";

const SearchPricing = () => {
  const [storeId, setStoreId] = useState("");
  const [sku, setSku] = useState("");
  const [records, setRecords] = useState([]);

  const handleSearch = async () => {
    const response = await axios.get("http://localhost:5000/api/pricing/search", {
      params: { storeId, sku },
    });
    setRecords(response.data);
  };

  return (
    <div>
      <h2>Search Pricing Data</h2>
      <input type="text" placeholder="Store ID" onChange={(e) => setStoreId(e.target.value)} />
      <input type="text" placeholder="SKU" onChange={(e) => setSku(e.target.value)} />
      <button onClick={handleSearch}>Search</button>

      <table>
        <thead>
          <tr>
            <th>Store ID</th>
            <th>SKU</th>
            <th>Product Name</th>
            <th>Price</th>
            <th>Date</th>
          </tr>
        </thead>
        <tbody>
          {records.map((record) => (
            <tr key={record.id}>
              <td>{record.storeId}</td>
              <td>{record.sku}</td>
              <td>{record.productName}</td>
              <td>{record.price}</td>
              <td>{record.date}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default SearchPricing;
