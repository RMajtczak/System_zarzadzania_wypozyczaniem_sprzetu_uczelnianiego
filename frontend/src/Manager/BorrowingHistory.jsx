import React, { useEffect, useState } from 'react';

function BorrowingHistory() {
    const [history, setHistory] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const token = localStorage.getItem('token');
        fetch('https://localhost:5001/api/borrowings', { // dostosuj URL do backendu
            headers: {
                'Authorization': `Bearer ${token}`
            }
        })
            .then(res => res.json())
            .then(data => {
                setHistory(data);
                setLoading(false);
            })
            .catch(err => {
                console.error("Błąd pobierania historii:", err);
                setLoading(false);
            });
    }, []);

    if (loading) return <p>Ładowanie historii...</p>;

    return (
        <div className="bg-white shadow-lg rounded-2xl p-6">
            <h2 className="text-xl font-bold mb-4">Historia wypożyczeń</h2>
            <table className="min-w-full divide-y divide-gray-200">
                <thead className="bg-gray-200">
                <tr>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                        Nazwa sprzętu
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                        Użytkownik
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                        Data rozpoczęcia
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                        Data zakończenia
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                        Stan
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                        Status zwrotu
                    </th>
                </tr>
                </thead>
                <tbody className="bg-white divide-y divide-gray-200">
                {history.length > 0 ? (
                    history.map((b) => (
                        <tr key={b.id} className="hover:bg-gray-50">
                            <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                                {b.equipmentName}
                            </td>
                            <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                                {b.borrowerName}
                            </td>
                            <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                                {new Date(b.startDate).toLocaleDateString()}
                            </td>
                            <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                                {b.endDate ? new Date(b.endDate).toLocaleDateString() : "-"}
                            </td>
                            <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                                {b.condition}
                            </td>
                            <td className="px-6 py-4 whitespace-nowrap">
                  <span
                      className={`px-2 inline-flex text-xs leading-5 font-semibold rounded-full ${
                          b.isReturned
                              ? "bg-green-100 text-green-800"
                              : "bg-red-100 text-red-800"
                      }`}
                  >
                    {b.isReturned ? "Zwrócony" : "W trakcie"}
                  </span>
                            </td>
                        </tr>
                    ))
                ) : (
                    <tr>
                        <td
                            colSpan="6"
                            className="px-6 py-4 text-center text-gray-500"
                        >
                            Brak historii wypożyczeń
                        </td>
                    </tr>
                )}
                </tbody>
            </table>
        </div>
    );
}

export default BorrowingHistory;
