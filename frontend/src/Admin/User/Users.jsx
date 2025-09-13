import React, { useEffect, useState } from "react";
import api from "../../api";

export default function UserList() {
    const [users, setUsers] = useState([]); // tablica użytkowników
    const [loading, setLoading] = useState(true);

    // Role na sztywno
    const [roles] = useState([
        { id: 1, name: "User" },
        { id: 2, name: "Manager" },
        { id: 3, name: "Admin" },
    ]);

    // Pobieranie użytkowników z backendu
    useEffect(() => {
        const fetchUsers = async () => {
            try {
                const res = await api.get("https://localhost:5001/api/users"); // pełny URL
                if (Array.isArray(res.data)) {
                    setUsers(res.data);
                } else {
                    console.error("Niepoprawny format danych z backendu:", res.data);
                    setUsers([]);
                }
            } catch (err) {
                console.error("Błąd przy pobieraniu użytkowników:", err);
                setUsers([]);
            } finally {
                setLoading(false);
            }
        };

        fetchUsers();
    }, []);

    // Zmiana roli użytkownika
    const handleRoleChange = async (userId, newRoleId) => {
        try {
            await api.put(`https://localhost:5001/api/users/update-role/${userId}`, {
                roleId: parseInt(newRoleId),
            });

            setUsers(prev =>
                prev.map(u => (u.id === userId ? { ...u, roleId: parseInt(newRoleId) } : u))
            );
        } catch (err) {
            console.error("Błąd przy aktualizacji roli:", err);
        }
    };

    if (loading) {
        return <p className="text-center text-gray-500">Ładowanie danych...</p>;
    }

    return (
        <div className="max-w-4xl mx-auto p-4">
            <table className="min-w-full divide-y divide-gray-200">
                <thead className="bg-gray-200">
                <tr>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                        Imię i nazwisko
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                        Email
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                        Rola
                    </th>
                </tr>
                </thead>
                <tbody className="bg-white divide-y divide-gray-200">
                {Array.isArray(users) && users.length > 0 ? (
                    users.map(user => (
                        <tr key={user.id} className="hover:bg-gray-50">
                            <td className="px-6 py-4 whitespace-nowrap">
                                <div className="text-sm font-medium text-gray-900">
                                    {user.firstName} {user.lastName}
                                </div>
                            </td>
                            <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                                {user.email}
                            </td>
                            <td className="px-6 py-4 whitespace-nowrap">
                                <select
                                    value={user.roleId || ""}
                                    onChange={e => handleRoleChange(user.id, e.target.value)}
                                    className="border px-2 py-1 rounded"
                                >
                                    {roles.map(role => (
                                        <option key={role.id} value={role.id}>
                                            {role.name}
                                        </option>
                                    ))}
                                </select>
                            </td>
                        </tr>
                    ))
                ) : (
                    <tr>
                        <td colSpan="3" className="px-6 py-4 text-center text-gray-500">
                            Brak użytkowników do wyświetlenia
                        </td>
                    </tr>
                )}
                </tbody>
            </table>
        </div>
    );
}
