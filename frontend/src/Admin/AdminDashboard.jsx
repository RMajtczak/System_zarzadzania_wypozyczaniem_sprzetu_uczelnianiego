import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import Equipment from './Equipment/Equipment';
import Faults from './Faults.jsx';
import Reports from './Reports.jsx';
import Settings from './Settings.jsx';
import Users from './Users';
import { FaUserCircle } from 'react-icons/fa';


function AdminDashboard() {
    const [activeTab, setActiveTab] = useState('equipment');
    const [showLogout, setShowLogout] = useState(false);
    const [userName, setUserName] = useState('');
    const navigate = useNavigate();

    useEffect(() => {
        const storedName = localStorage.getItem('userName');
        setUserName(storedName || 'Nieznany użytkownik');
    }, []);

    const handleLogout = () => {
        localStorage.removeItem('token');
        localStorage.removeItem('userName');
        navigate('/login');
    };

    const renderContent = () => {
        switch (activeTab) {
            case 'equipment':
                return <Equipment/>;
            case 'faults':
                return <Faults/>;
            case 'reports':
                return <Reports/>;
            case 'settings':
                return <Settings/>;
            case 'users':
                return <Users/>;
            default:
                return null;
        }
    };

    return (
        <div className="min-h-screen font-sans bg-gray-100">
            <header className="bg-blue-600 text-white py-4 shadow-md relative">
                <div className="flex items-center justify-center space-x-10">
                    <button
                        className={`hover:underline ${activeTab === 'equipment' ? 'font-bold' : ''} cursor-pointer`}
                        onClick={() => setActiveTab('equipment')}
                    >
                        Zarządzanie sprzętem
                    </button>
                    <button
                        className={`hover:underline ${activeTab === 'users' ? 'font-bold' : ''} cursor-pointer`}
                        onClick={() => setActiveTab('users')}
                    >
                        Zarządzanie użytkownikami
                    </button>
                    <button
                        className={`hover:underline ${activeTab === 'faults' ? 'font-bold' : ''} cursor-pointer`}
                        onClick={() => setActiveTab('faults')}
                    >
                        Zarządzanie usterkami
                    </button>
                    <button
                        className={`hover:underline ${activeTab === 'reports' ? 'font-bold' : ''} cursor-pointer`}
                        onClick={() => setActiveTab('reports')}
                    >
                        Raporty
                    </button>
                    <button
                        className={`hover:underline ${activeTab === 'settings' ? 'font-bold' : ''} cursor-pointer`}
                        onClick={() => setActiveTab('settings')}
                    >
                        Ustawienia
                    </button>

                </div>

                <div className="absolute right-6 top-1/2 transform -translate-y-1/2">
                    <div className="relative">
                        <button
                            onClick={() => setShowLogout(!showLogout)}
                            className="flex items-center space-x-2 text-sm hover:underline cursor-pointer"
                        >
                            <FaUserCircle className="text-xl" />
                            <span>{userName}</span>
                        </button>
                        {showLogout && (
                            <div className="absolute right-0 mt-2 bg-white text-black rounded-lg shadow-lg py-2 px-4 z-50">
                                <button
                                    onClick={handleLogout}
                                    className="hover:text-red-600 cursor-pointer transition-colors"
                                >
                                    Wyloguj się
                                </button>
                            </div>
                        )}
                    </div>
                </div>
            </header>

            <main className="max-w-6xl mx-auto py-8 px-4">
                <h1 className="text-center text-2xl font-semibold mb-6">
                    {activeTab === 'users' && 'Users'}
                    {activeTab === 'faults' && 'Faults'}
                    {activeTab === 'reports' && 'Reports'}
                    {activeTab === 'settings' && 'Settings'}
                </h1>
                {renderContent()}
            </main>
        </div>
    );
}

export default AdminDashboard;
