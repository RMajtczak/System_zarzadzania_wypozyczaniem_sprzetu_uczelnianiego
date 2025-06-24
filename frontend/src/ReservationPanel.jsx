import React, { useState, useEffect } from "react";
import axios from "axios";

const ReservationPanel = () => {
    const [equipmentList, setEquipmentList] = useState([]);
    const [selectedEquipmentId, setSelectedEquipmentId] = useState(null);
    const [startDate, setStartDate] = useState("");
    const [endDate, setEndDate] = useState("");
    const [message, setMessage] = useState("");

    useEffect(() => {
        axios
            .get("https://localhost:5001/api/equipment")
            .then((res) => setEquipmentList(res.data))
            .catch((err) => console.error(err));
    }, []);

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
        axios.post("https://localhost:5001/api/reservations", reservationData, {
            headers: {
                "Content-Type": "application/json",
                Authorization: `Bearer ${token}`,
            },
        })
            .then(() => setMessage("Rezerwacja dodana pomyślnie!"))
            .catch(error => {
                if (error.response) {
                    setMessage(`Błąd: ${error.response.status} - ${error.response.data}`);
                } else {
                    setMessage("Błąd podczas rezerwacji.");
                }
            });
    };

    return (
        <div className="max-w-4xl mx-auto p-4">
            <h2 className="text-2xl font-semibold mb-4">Rezerwacja Sprzętu</h2>

            <div className="space-y-4">
                {equipmentList.map((eq) => (
                    <div
                        key={eq.id}
                        className={`p-4 border rounded-lg cursor-pointer ${
                            selectedEquipmentId === eq.id ? "bg-blue-100" : "bg-white"
                        }`}
                        onClick={() => setSelectedEquipmentId(eq.id)}
                    >
                        <h3 className="font-bold">
                            {eq.name} ({eq.type})
                        </h3>
                        <p>Lokalizacja: {eq.location}</p>
                        <p className="text-sm text-gray-600">{eq.specification}</p>
                    </div>
                ))}
            </div>

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

            <button
                onClick={handleReservation}
                className="mt-6 bg-blue-600 hover:bg-blue-700 text-white px-6 py-2 rounded"
            >
                Zarezerwuj
            </button>

            {message && <p className="mt-4 text-center text-red-500">{message}</p>}
        </div>
    );
};

export default ReservationPanel;
