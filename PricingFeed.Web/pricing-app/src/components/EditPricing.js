import { useState } from "react";
import axios from "axios";

const EditPricing = ({ record }) => {
  const [price, setPrice] = useState(record.price);
  const [date, setDate] = useState(record.date);

  const handleSave = async () => {
    try {
      await axios.put(`http://localhost:5000/api/pricing/edit/${record.id}`, {
        price,
        date,
      });
      alert("Updated successfully");
    } catch (error) {
      alert("Failed to update");
    }
  };

  return (
    <tr>
      <td>{record.storeId}</td>
      <td>{record.sku}</td>
      <td>{record.productName}</td>
      <td>
        <input type="number" value={price} onChange={(e) => setPrice(e.target.value)} />
      </td>
      <td>
        <input type="date" value={date} onChange={(e) => setDate(e.target.value)} />
      </td>
      <td>
        <button onClick={handleSave}>Save</button>
      </td>
    </tr>
  );
};

export default EditPricing;
