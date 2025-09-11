import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import Equipment from './Equipment/Equipment'; 
import BorrowingPanel from './BorrowingPanel';
import BorrowingHistory from './BorrowingHistory';
import { FaUserCircle } from 'react-icons/fa';

function ManagerDashboard() {
    const [activeTab, setActiveTab] = useState('equipment');
    const [showLogout, setShowLogout] = useState(false);
    const [userName, setUserName] = useState('');
    const navigate = useNavigate();

    useEffect(() => {
        const storedName = localStorage.getItem('userName');
        setUserName(storedName || 'Nieznany opiekun');
    }, []);

    const handleLogout = () => {
        localStorage.removeItem('token');
        localStorage.removeItem('userName');
        navigate('/login');
    };

    const renderContent = () => {
        switch (activeTab) {
            case 'equipment':
                return <Equipment />;
            case 'borrowing':
                return <BorrowingPanel />;
            case 'history':
                return <BorrowingHistory />;
            default:
                return null;
        }
    };

    return (
        <div className="min-h-screen font-sans bg-gray-100">
            <header className="bg-green-600 text-white py-4 shadow-md relative">
                <div className="flex items-center justify-center space-x-10">
                    <button
                        className={`hover:underline ${activeTab === 'equipment' ? 'font-bold' : ''} cursor-pointer`}
                        onClick={() => setActiveTab('equipment')}
                    >
                        Zarządzanie sprzętem
                    </button>
                    <button
                        className={`hover:underline ${activeTab === 'borrowing' ? 'font-bold' : ''} cursor-pointer`}
                        onClick={() => setActiveTab('borrowing')}
                    >
                        Wypożyczanie sprzętu
                    </button>
                    <button
                        className={`hover:underline ${activeTab === 'history' ? 'font-bold' : ''} cursor-pointer`}
                        onClick={() => setActiveTab('history')}
                    >
                        Historia wypożyczeń
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
                {renderContent()}
            </main>
        </div>
    );
}

export default ManagerDashboard;
