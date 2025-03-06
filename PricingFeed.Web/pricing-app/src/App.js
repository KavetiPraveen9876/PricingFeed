import { BrowserRouter as Router, Route, Routes } from "react-router-dom";
import { AuthProvider } from "./context/AuthContext";
import Login from "./Pages/Login";
import Dashboard from "./Pages/Dashboard";
import UploadCsv from "./components/UploadCsv";
import SearchPricing from "./components/SearchPricing";

function App() {
  return (
    <AuthProvider>
      <Router>
        <Routes>
          <Route path="/" element={<Login />} />
          <Route path="/dashboard" element={<Dashboard />} />
          <Route path="/upload" element={<UploadCsv />} />
          <Route path="/search" element={<SearchPricing />} />
        </Routes>
      </Router>
    </AuthProvider>
  );
}

export default App;
