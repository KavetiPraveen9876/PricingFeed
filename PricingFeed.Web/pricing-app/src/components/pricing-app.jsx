import React, { useState, useEffect } from 'react';
import axios from 'axios';
import './pricing-app.css';

function PricingApp() {
  const [file, setFile] = useState(null);
  const [pricingData, setPricingData] = useState([]);
  const [searchCriteria, setSearchCriteria] = useState({
    storeId: '',
    sku: '',
    date: '',
  });
  const [editRow, setEditRow] = useState(null);

  const handleFileChange = (e) => {
    setFile(e.target.files[0]);
  };

  const handleUploadSubmit = async (e) => {
    e.preventDefault();
    const formData = new FormData();
    formData.append('File', file);
    await axios.post('/api/pricing/upload', formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    });
    alert('File uploaded successfully!');
    fetchPricingData(); // Refresh data after upload
  };

  const handleSearchChange = (e) => {
    setSearchCriteria({ ...searchCriteria, [e.target.name]: e.target.value });
  };

  const handleSearchSubmit = async (e) => {
    e.preventDefault();
    fetchPricingData();
  };

  const fetchPricingData = async () => {
    try {
      const response = await axios.get('/api/pricing', {
        params: searchCriteria,
      });
      setPricingData(response.data);
    } catch (error) {
      console.error('Error fetching data:', error);
    }
  };

  const handleEdit = (row) => {
    setEditRow(row.id);
  };

  const handleSave = async (updatedRow) => {
    try {
      await axios.put(`/api/pricing/${updatedRow.id}`, updatedRow);
      setEditRow(null);
      fetchPricingData(); // Refresh data after save
    } catch (error) {
      console.error('Error updating data:', error);
    }
  };

  const handleInputChange = (e, row) => {
    const updatedData = pricingData.map((item) => {
      if (item.id === row.id) {
        return { ...item, [e.target.name]: e.target.value };
      }
      return item;
    });
    setPricingData(updatedData);
  };

  useEffect(() => {
    //fetchPricingData();
  }, []);

  return (
    <div className="pricing-app">
      <h2>Pricing Feed Management</h2>

      {/* Upload Section */}
      <div className="section">
        <h3>Upload CSV</h3>
        <form onSubmit={handleUploadSubmit}>
          <input type="file" onChange={handleFileChange} />
          <button type="submit">Upload</button>
        </form>
      </div>

      {/* Search Section */}
      <div className="section">
        <h3>Search</h3>
        <form onSubmit={handleSearchSubmit}>
          <input
            type="text"
            name="storeId"
            placeholder="Store ID"
            value={searchCriteria.storeId}
            onChange={handleSearchChange}
          />
          <input
            type="text"
            name="sku"
            placeholder="SKU"
            value={searchCriteria.sku}
            onChange={handleSearchChange}
          />
          <input
            type="date"
            name="date"
            value={searchCriteria.date}
            onChange={handleSearchChange}
          />
          <button type="submit">Search</button>
        </form>
      </div>

      {/* Data Display and Edit Section */}
      <div className="section">
        <h3>Pricing Data</h3>
        <table>
          <thead>
            <tr>
              <th>Store ID</th>
              <th>SKU</th>
              <th>Product Name</th>
              <th>Price</th>
              <th>Date</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {pricingData.map((row) => (
              <tr key={row.id}>
                {/* ... (table data remains the same) */}
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}

export default PricingApp;