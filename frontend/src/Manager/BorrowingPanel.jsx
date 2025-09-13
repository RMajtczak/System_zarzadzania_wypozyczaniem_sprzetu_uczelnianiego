import React, { useState, useEffect } from 'react';
import api from '../api';

function BorrowingPanel() {
    const [equipment, setEquipment] = useState([]);
    const [users, setUsers] = useState([]);
    const [borrowings, setBorrowings] = useState([]);
    const [selectedEquipment, setSelectedEquipment] = useState('');
    const [selectedUser, setSelectedUser] = useState('');
    const [selectedCondition, setSelectedCondition] = useState('Dobry'); 
    const [loading, setLoading] = useState(false);
    const token = localStorage.getItem('token');

    useEffect(() => {
        fetchEquipment();
        fetchUsers();
        fetchBorrowings();
    }, []);

    const fetchEquipment = async () => {
        try {
            const res = await api.get('https://localhost:5001/api/equipment', {
                headers: { Authorization: `Bearer ${token}` },
            });
            // upewniamy się, że zawsze mamy tablicę
            const data = Array.isArray(res.data) ? res.data : res.data?.equipment || [];
            setEquipment(data);
        } catch (err) {
            console.error('Błąd przy pobieraniu sprzętu:', err);
        }
    };

    const fetchUsers = async () => {
        try {
            const res = await api.get('https://localhost:5001/api/users', {
                headers: { Authorization: `Bearer ${token}` },
            });
            const data = Array.isArray(res.data) ? res.data : res.data?.users || [];
            setUsers(data);
        } catch (err) {
            console.error('Błąd przy pobieraniu użytkowników:', err);
        }
    };

    const fetchBorrowings = async () => {
        try {
            const res = await api.get('https://localhost:5001/api/borrowings/active', {
                headers: { Authorization: `Bearer ${token}` },
            });
            const data = Array.isArray(res.data) ? res.data : [];
            setBorrowings(data);
        } catch (err) {
            console.error('Błąd przy pobieraniu wypożyczeń:', err);
        }
    };

    const handleBorrow = async () => {
        if (!selectedEquipment || !selectedUser) {
            alert('Wybierz sprzęt i użytkownika!');
            return;
        }
        setLoading(true);
        try {
            await api.post(
                'https://localhost:5001/api/borrowings',
                {
                    equipmentName: selectedEquipment,
                    borrowerName: selectedUser,
                    condition: selectedCondition, 
                    startDate: new Date().toISOString(),
                    endDate: new Date(new Date().setDate(new Date().getDate() + 7)).toISOString(),
                },
                { headers: { Authorization: `Bearer ${token}` } }
            );
            alert('Sprzęt wypożyczony!');
            fetchBorrowings();
        } catch (err) {
            console.error('Błąd przy wypożyczaniu:', err.response?.data || err.message);
            alert('Błąd przy wypożyczaniu: ' + (err.response?.data || err.message));
        } finally {
            setLoading(false);
        }
    };


    const handleReturn = async (id) => {
        if (!window.confirm('Czy na pewno chcesz zwrócić ten sprzęt?')) return;
        try {
            await api.patch(`https://localhost:5001/api/borrowings/${id}`, null, {
                headers: { Authorization: `Bearer ${token}` },
            });
            alert('Sprzęt został zwrócony.');
            fetchBorrowings();
        } catch (err) {
            console.error('Błąd przy zwracaniu sprzętu:', err);
        }
    };

    return (
        <div className="bg-white p-6 rounded-xl shadow-md">
            <h2 className="text-xl font-bold mb-4">Panel wypożyczeń</h2>

            <div className="mb-6">
                <label className="block mb-2 font-medium">Wybierz sprzęt</label>
                <select
                    value={selectedEquipment}
                    onChange={(e) => setSelectedEquipment(e.target.value)}
                    className="border p-2 rounded w-full"
                >
                    <option value="">-- Wybierz sprzęt --</option>
                    {Array.isArray(equipment) &&
                        equipment.map(item => (
                            <option key={item.id} value={item.name}>
                                {item.name} ({item.location})
                            </option>
                        ))}
                </select>
                <label className="block mt-4 mb-2 font-medium">Stan sprzętu</label>
                <select
                    value={selectedCondition}
                    onChange={(e) => setSelectedCondition(e.target.value)}
                    className="border p-2 rounded w-full"
                >
                    <option value="Dobry">Dobry</option>
                    <option value="Średni">Średni</option>
                    <option value="Zły">Zły</option>
                </select>
                <label className="block mt-4 mb-2 font-medium">Wybierz użytkownika</label>
                <select
                    value={selectedUser}
                    onChange={(e) => setSelectedUser(e.target.value)}
                    className="border p-2 rounded w-full"
                >
                    <option value="">-- Wybierz użytkownika --</option>
                    {Array.isArray(users) &&
                        users.map(user => (
                            <option key={user.id} value={`${user.firstName} ${user.lastName}`}>
                                {user.firstName} {user.lastName}
                            </option>
                        ))}
                </select>

                <button
                    onClick={handleBorrow}
                    disabled={loading}
                    className={`mt-4 w-full bg-green-600 text-white py-2 rounded-lg hover:bg-green-700 transition ${loading && 'opacity-50'}`}
                >
                    {loading ? 'Wypożyczanie...' : 'Wypożycz sprzęt'}
                </button>
            </div>

            {/* Lista aktywnych wypożyczeń */}
            <div>
                <h3 className="text-lg font-semibold mb-2">Aktywne wypożyczenia</h3>
                {borrowings.length === 0 ? (
                    <p className="text-gray-600">Brak aktywnych wypożyczeń</p>
                ) : (
                    <ul className="space-y-2">
                        {borrowings.map(borrowing => (
                            <li
                                key={borrowing.id}
                                className="flex justify-between items-center bg-gray-100 p-2 rounded"
                            >
                                <span>
                                    <strong>{borrowing.equipmentName}</strong> → Użytkownik: {borrowing.borrowerName}
                                </span>
                                {!borrowing.isReturned && (
                                    <button
                                        onClick={() => handleReturn(borrowing.id)}
                                        className="bg-blue-500 text-white px-3 py-1 rounded hover:bg-blue-600"
                                    >
                                        Zwróć
                                    </button>
                                )}
                            </li>
                        ))}
                    </ul>
                )}
            </div>
        </div>
    );
}

export default BorrowingPanel;
