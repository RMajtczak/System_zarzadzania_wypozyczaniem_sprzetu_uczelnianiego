import React, { useEffect, useState } from "react";
import axios from "axios";

export default function FaultReports() {
    const [faults, setFaults] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        fetchFaults();
    }, []);

    const fetchFaults = async () => {
        try {
            const res = await axios.get("https://localhost:5001/api/faultreports");
            if (Array.isArray(res.data)) {
                setFaults(res.data);
            } else {
                console.error("Niepoprawny format danych z backendu:", res.data);
                setFaults([]);
            }
        } catch (err) {
            console.error("Błąd przy pobieraniu usterek:", err);
            setFaults([]);
        } finally {
            setLoading(false);
        }
    };

    const handleResolve = async (id) => {
        try {
            await axios.patch(`https://localhost:5001/api/faultreports/${id}/resolve`);
            setFaults(prev =>
                prev.map(f => (f.id === id ? { ...f, isResolved: true } : f))
            );
        } catch (err) {
            console.error("Błąd przy rozwiązywaniu usterki:", err);
        }
    };

    if (loading) {
        return <p className="text-center text-gray-500">Ładowanie usterek...</p>;
    }

    return (
        <div className="max-w-4xl mx-auto p-4">
            <h2 className="text-xl font-semibold mb-4">Lista usterek</h2>
            <table className="min-w-full divide-y divide-gray-200">
                <thead className="bg-gray-200">
                <tr>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                        Sprzęt
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                        Zgłaszający
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                        Opis
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                        Data zgłoszenia
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                        Status
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                        Akcje
                    </th>
                </tr>
                </thead>
                <tbody className="bg-white divide-y divide-gray-200">
                {faults.length > 0 ? (
                    faults.map(f => (
                        <tr key={f.id} className="hover:bg-gray-50">
                            <td className="px-6 py-4 whitespace-nowrap">{f.equipmentName || "Brak danych"}</td>
                            <td className="px-6 py-4 whitespace-nowrap">{f.userName || "Brak danych"}</td>
                            <td className="px-6 py-4 whitespace-nowrap">{f.description}</td>
                            <td className="px-6 py-4 whitespace-nowrap">{new Date(f.reportDate).toLocaleString()}</td>
                            <td className="px-6 py-4 whitespace-nowrap">{f.isResolved ? "Rozwiązane" : "Otwarte"}</td>
                            <td className="px-6 py-4 whitespace-nowrap">
                                {!f.isResolved && (
                                    <button
                                        onClick={() => handleResolve(f.id)}
                                        className="bg-green-500 text-white px-2 py-1 rounded hover:bg-green-600"
                                    >
                                        Rozwiąż
                                    </button>
                                )}
                            </td>
                        </tr>
                    ))
                ) : (
                    <tr>
                        <td colSpan="6" className="px-6 py-4 text-center text-gray-500">
                            Brak zgłoszonych usterek
                        </td>
                    </tr>
                )}
                </tbody>
            </table>
        </div>
    );
}
