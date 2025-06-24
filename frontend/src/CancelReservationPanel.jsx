import React, { useEffect, useState } from "react";
import axios from "axios";

const CancelReservationPanel = () => {
    const [reservations, setReservations] = useState([]);
    const [message, setMessage] = useState("");

    const token = localStorage.getItem("token");

    useEffect(() => {
        axios
            .get("https://localhost:5001/api/reservations", {
                headers: {
                    Authorization: `Bearer ${token}`,
                },
            })
            .then((res) => setReservations(res.data))
            .catch((error) => {
                const errMsg = error.response?.data?.message || "Błąd podczas pobierania rezerwacji.";
                setMessage(errMsg);
            });
    }, [token]);

    const handleCancel = (id) => {
        axios
            .post(`https://localhost:5001/api/reservations/${id}/cancel`, null, {
                headers: {
                    Authorization: `Bearer ${token}`,
                },
            })
            .then(() => {
                setMessage("Rezerwacja anulowana.");
                setReservations((prev) => prev.filter((r) => r.id !== id));
            })
            .catch((error) => {
                const errMsg = error.response?.data?.message || "Błąd podczas anulowania rezerwacji.";
                setMessage(errMsg);
            });
    };

    return (
        <div className="max-w-3xl mx-auto p-4">
            <h2 className="text-2xl font-bold mb-4">Twoje rezerwacje</h2>

            {message && <p className="text-red-500 mb-4">{message}</p>}

            {reservations.length === 0 ? (
                <p>Brak aktywnych rezerwacji.</p>
            ) : (
                <ul className="space-y-4">
                    {reservations.map((res) => (
                        <li
                            key={res.id}
                            className="bg-white p-4 border rounded shadow flex justify-between items-center"
                        >
                            <div>
                                <p className="font-semibold">{res.equipmentName}</p>
                                <p className="text-sm text-gray-600">
                                    {new Date(res.startDate).toLocaleDateString()} -{" "}
                                    {new Date(res.endDate).toLocaleDateString()}
                                </p>
                            </div>
                            <button
                                onClick={() => handleCancel(res.id)}
                                className="bg-red-600 hover:bg-red-700 text-white px-4 py-2 rounded cursor-pointer"
                            >
                                Anuluj
                            </button>
                        </li>
                    ))}
                </ul>
            )}
        </div>
    );
};

export default CancelReservationPanel;
