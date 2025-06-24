import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import LoginPage from './LoginPage';
import RegisterPage from './RegisterPage';
import UserDashboard from "./UserDashboard.jsx";

function App() {
    return (
        <Router>
            <Routes>
                <Route path="/login" element={<LoginPage />} />
                <Route path="/register" element={<RegisterPage />} />
                <Route path="*" element={<LoginPage />} /> {/* domyślnie login */}
                <Route path="/user-dashboard" element={<UserDashboard />} />
            </Routes>
        </Router>
    );
}

export default App;