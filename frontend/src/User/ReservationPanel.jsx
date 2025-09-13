import React, { useState, useEffect } from "react";
import api from "../api";

const ReservationPanel = () => {
    const [equipmentList, setEquipmentList] = useState([]);
    const [selectedEquipmentId, setSelectedEquipmentId] = useState(null);
    const [startDate, setStartDate] = useState("");
    const [endDate, setEndDate] = useState("");
    const [message, setMessage] = useState("");
    const [search, setSearch] = useState("");

    useEffect(() => {
        fetchEquipments();
    }, []);

    const fetchEquipments = (query = "") => {
        const url = query
            ? `https://localhost:5001/api/equipment/search?name=${encodeURIComponent(query)}`
            : `https://localhost:5001/api/equipment`;

        api
            .get(url)
            .then((res) => setEquipmentList(res.data))
            .catch((err) => {
                console.error(err);
                setEquipmentList([]);
            });
    };

    const handleSearch = (e) => {
        e.preventDefault();
        fetchEquipments(search);
    };

    const handleReservation = () => {
        if (!selectedEquipmentId || !startDate || !endDate) {
            setMessage("Uzupełnij wszystkie pola");
            return;
        }

        const selectedEquipment = equipmentList.find((eq) => eq.id === selectedEquipmentId);
        if (!selectedEquipment) {
            setMessage("Wybrano nieprawidłowy sprzęt");
            return;
        }

        if (selectedEquipment.status !== "Dostępny") {
            setMessage("Ten sprzęt nie jest dostępny do rezerwacji.");
            return;
        }

        const bookerName = localStorage.getItem("userName") || "Nieznany użytkownik";

        const reservationData = {
            bookerName: bookerName,
            equipmentName: selectedEquipment.name,
            startDate: new Date(startDate).toISOString(),
            endDate: new Date(endDate).toISOString(),
        };

        const token = localStorage.getItem("token");
        if (!token) {
            setMessage("Brak tokenu autoryzacyjnego");
            return;
        }

        api.post("https://localhost:5001/api/reservations", reservationData, {
            headers: {
                "Content-Type": "application/json",
                Authorization: `Bearer ${token}`,
            },
        })
            .then(() => {
                setMessage("Rezerwacja dodana pomyślnie!");
                setEquipmentList(equipmentList.filter((eq) => eq.id !== selectedEquipmentId));
                setSelectedEquipmentId(null);
                setStartDate("");
                setEndDate("");
            })
            .catch((error) => {
                if (error.response) {
                    setMessage(`Błąd: ${error.response.status} - ${JSON.stringify(error.response.data)}`);
                } else {
                    setMessage("Błąd podczas rezerwacji.");
                }
            });
    };

    return (
        <div className="max-w-5xl mx-auto p-4">
            {/* Wyszukiwanie */}
            <form onSubmit={handleSearch} className="flex gap-2 mb-4">
                <input
                    type="text"
                    placeholder="Szukaj sprzętu..."
                    value={search}
                    onChange={(e) => setSearch(e.target.value)}
                    className="border p-2 rounded w-full"
                />
                <button type="submit" className="bg-blue-500 text-white px-4 py-2 rounded">
                    Szukaj
                </button>
            </form>

            {/* Tabela sprzętu */}
            <table className="min-w-full divide-y divide-gray-200">
                <thead className="bg-gray-200">
                <tr>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Nazwa</th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Typ</th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Lokalizacja</th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Specyfikacja</th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Status</th>
                    <th className="px-6 py-3 text-center text-xs font-medium text-gray-500 uppercase tracking-wider">Wybór</th>
                </tr>
                </thead>
                <tbody className="bg-white divide-y divide-gray-200">
                {equipmentList.length > 0 ? (
                    equipmentList.map((eq) => (
                        <tr
                            key={eq.id}
                            className={`hover:bg-gray-50 ${
                                eq.status !== "Dostępny" ? "opacity-50" : ""
                            }`}
                        >
                            <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">{eq.name}</td>
                            <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">{eq.type}</td>
                            <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">{eq.location}</td>
                            <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">{eq.specification}</td>
                            <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">{eq.status}</td>
                            <td className="px-6 py-4 whitespace-nowrap text-center">
                                <input
                                    type="radio"
                                    name="selectedEquipment"
                                    checked={selectedEquipmentId === eq.id}
                                    onChange={() => setSelectedEquipmentId(eq.id)}
                                    disabled={eq.status !== "Dostępny"}
                                />
                            </td>
                        </tr>
                    ))
                ) : (
                    <tr>
                        <td colSpan="6" className="px-6 py-4 text-center text-gray-500">
                            Brak sprzętu do wyświetlenia
                        </td>
                    </tr>
                )}
                </tbody>
            </table>

            {/* Daty rezerwacji */}
            <div className="mt-6 grid grid-cols-1 md:grid-cols-2 gap-4">
                <div>
                    <label className="block text-sm font-medium mb-1">Data rozpoczęcia</label>
                    <input
                        type="date"
                        value={startDate}
                        onChange={(e) => setStartDate(e.target.value)}
                        className="w-full border rounded px-3 py-2"
                    />
                </div>

                <div>
                    <label className="block text-sm font-medium mb-1">Data zakończenia</label>
                    <input
                        type="date"
                        value={endDate}
                        onChange={(e) => setEndDate(e.target.value)}
                        className="w-full border rounded px-3 py-2"
                    />
                </div>
            </div>

            {/* Przycisk rezerwacji */}
            <button
                onClick={handleReservation}
                className="mt-6 bg-blue-600 hover:bg-blue-700 text-white px-6 py-2 rounded"
            >
                Zarezerwuj
            </button>

            {/* Komunikat */}
            {message && <p className="mt-4 text-center text-red-500">{message}</p>}
        </div>
    );
};

export default ReservationPanel;
