import React, { useEffect, useState } from "react";
import api from "../api";

const CancelReservationPanel = () => {
    const [reservations, setReservations] = useState([]);
    const [message, setMessage] = useState("");

    const token = localStorage.getItem("token");

    const fetchReservations = () => {
        api.get("http://localhost:5000/api/reservations/active", {
            headers: {
                Authorization: `Bearer ${token}`,
            },
        })

            .then((res) => {
                const now = new Date().getTime();
                const activeReservations = res.data.filter(
                    (r) => new Date(r.endDate).getTime() >= now
                );
                setReservations(activeReservations);
            })
            .catch((error) => {
                const errMsg =
                    error.response?.data?.message ||
                    "Błąd podczas pobierania rezerwacji.";
                setMessage(errMsg);
            });
    };

    useEffect(() => {
        fetchReservations();
    }, [token]);

    const handleCancel = (id) => {
        const now = new Date().getTime();

        api
            .post(`http://localhost:5000/api/reservations/${id}/cancel`, null, {
                headers: {
                    Authorization: `Bearer ${token}`,
                },
            })
            .then(() => {
                setMessage("Rezerwacja anulowana.");
                setReservations((prev) =>
                    prev.filter((r) => r.id !== id && new Date(r.endDate).getTime() >= now)
                );
            })
            .catch((error) => {
                const errMsg =
                    error.response?.data?.message ||
                    "Błąd podczas anulowania rezerwacji.";
                setMessage(errMsg);
            });
    };

    return (
        <div className="max-w-4xl mx-auto p-4">
            <h2 className="text-2xl font-bold mb-4 text-center">Twoje aktywne rezerwacje</h2>

            {message && <p className="text-red-500 mb-4 text-center">{message}</p>}

            {reservations.length === 0 ? (
                <p className="text-center text-gray-500">Brak aktywnych rezerwacji.</p>
            ) : (
                <table className="min-w-full divide-y divide-gray-200">
                    <thead className="bg-gray-200">
                    <tr>
                        <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                            Sprzęt
                        </th>
                        <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                            Okres rezerwacji
                        </th>
                        <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                            Akcje
                        </th>
                    </tr>
                    </thead>
                    <tbody className="bg-white divide-y divide-gray-200">
                    {reservations.map((res) => (
                        <tr key={res.id} className="hover:bg-gray-50">
                            <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                                {res.equipmentName}
                            </td>
                            <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                                {new Date(res.startDate).toLocaleDateString()} -{" "}
                                {new Date(res.endDate).toLocaleDateString()}
                            </td>
                            <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                                <button
                                    onClick={() => handleCancel(res.id)}
                                    className="bg-red-600 hover:bg-red-700 text-white px-4 py-2 rounded"
                                >
                                    Anuluj
                                </button>
                            </td>
                        </tr>
                    ))}
                    </tbody>
                </table>
            )}
        </div>
    );
};

export default CancelReservationPanel;
