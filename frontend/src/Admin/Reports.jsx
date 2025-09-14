import { useEffect, useState } from "react";
import api from "../api"; 


function safeGet(obj, names) {
    if (!obj) return null;
    for (const n of names) {
        if (obj[n] !== undefined && obj[n] !== null) return obj[n];
    }
    return null;
}

function parseDate(raw) {
    if (!raw && raw !== 0) return null;
    
    if (raw instanceof Date) {
        if (isNaN(raw.getTime())) return null;
        return raw;
    }
    
    if (typeof raw === "number") {
        const d = new Date(raw);
        return isNaN(d.getTime()) ? null : d;
    }

    try {
        const msMatch = /\/Date\((\d+)(?:[+-]\d+)?\)\//.exec(raw);
        if (msMatch) {
            const d = new Date(parseInt(msMatch[1], 10));
            return isNaN(d.getTime()) ? null : d;
        }
        
        const d = new Date(raw);
        if (!isNaN(d.getTime())) return d;
        
        const alt = raw.replace(" ", "T");
        const d2 = new Date(alt);
        return isNaN(d2.getTime()) ? null : d2;
    } catch {
        return null;
    }
}

function formatDateForUI(raw) {
    const d = parseDate(raw);
    if (!d) return "-";
    return d.toLocaleDateString(); // możesz dostosować format
}

export default function ReportsPanel() {
    const [borrowings, setBorrowings] = useState([]);
    const [topEquipment, setTopEquipment] = useState([]);

    useEffect(() => {
        // Pobieranie wypożyczeń (JSON endpoint musi istnieć)
        api.get("http://localhost:5000/api/reports/borrowings")
            .then(res => {
                const data = Array.isArray(res.data) ? res.data : [];
                const mapped = data.map(b => {
                    const user = safeGet(b, ["user", "User", "fullName", "FullName"]);
                    const first = safeGet(b, ["firstName", "FirstName"]);
                    const last  = safeGet(b, ["lastName", "LastName"]);
                    const resolvedUser = user ?? (first && last ? `${first} ${last}` : "Unknown");

                    const equipment = safeGet(b, ["equipment", "Equipment", "equipmentName", "EquipmentName", "name", "Name"]) ?? "Unknown";

                    const startDateRaw = safeGet(b, ["startDate", "StartDate", "borrowDate", "BorrowDate"]);
                    const endDateRaw   = safeGet(b, ["endDate", "EndDate", "returnDate", "ReturnDate"]);

                    const isReturned = safeGet(b, ["isReturned", "IsReturned"]) || false;

                    return {
                        user: resolvedUser,
                        equipment,
                        startDateRaw,
                        endDateRaw,
                        startDateFormatted: formatDateForUI(startDateRaw),
                        endDateFormatted: formatDateForUI(endDateRaw),
                        isReturned
                    };
                });

                setBorrowings(mapped);
            })
            .catch(err => {
                console.error("Error loading borrowings:", err);
                setBorrowings([]);
            });
        
        api.get("http://localhost:5000/api/reports/top-equipment?topN=5")
            .then(res => {
                const arr = Array.isArray(res.data) ? res.data : [];
                const mapped = arr.map(i => {
                    const name = safeGet(i, ["equipment", "Equipment", "EquipmentName", "equipmentName", "name", "Name"]) ?? safeGet(i, ["key", "Key"]) ?? "Unknown";
                    const count = safeGet(i, ["count", "Count", "value", "Value"]) ?? 0;
                    return { equipment: name, count };
                });
                setTopEquipment(mapped);
            })
            .catch(err => {
                console.error("Error loading top equipment:", err);
                setTopEquipment([]);
            });
    }, []);

    const downloadCsv = (endpoint) => {
        window.open(`http://localhost:5000/api/reports/${endpoint}/csv`, "_blank");
    };

    const downloadPdf = (endpoint) => {
        window.open(`http://localhost:5000/api/reports/${endpoint}/pdf`, "_blank");
    };

    return (
        <div className="p-6 space-y-8">
            {/* Borrowings */}
            <div className="bg-white shadow rounded-2xl p-4">
                <div className="flex justify-between items-center mb-4">
                    <h2 className="text-xl font-bold">Raport wypożyczeń</h2>
                    <div className="space-x-2">
                        <button onClick={() => downloadCsv("borrowings")} className="bg-blue-500 text-white px-4 py-2 rounded">Export CSV</button>
                        <button onClick={() => downloadPdf("borrowings")} className="bg-red-500 text-white px-4 py-2 rounded">Export PDF</button>
                    </div>
                </div>

                <div className="overflow-x-auto">
                    <table className="w-full border">
                        <thead className="bg-gray-100">
                        <tr>
                            <th className="p-2 border">Użytkownik</th>
                            <th className="p-2 border">Przedmiot</th>
                            <th className="p-2 border">Początek wypożyczenia</th>
                            <th className="p-2 border">Koniec wypożyczenia</th>
                            <th className="p-2 border">Status</th>
                        </tr>
                        </thead>
                        <tbody>
                        {borrowings.map((b, i) => (
                            <tr key={i} className="border-t hover:bg-gray-50">
                                <td className="p-2 border">{b.user}</td>
                                <td className="p-2 border">{b.equipment}</td>
                                <td className="p-2 border">{b.startDateFormatted}</td>
                                <td className="p-2 border">{b.endDateFormatted}</td>
                                <td className="p-2 border">{b.isReturned ? "Returned" : "Active"}</td>
                            </tr>
                        ))}
                        </tbody>
                    </table>
                </div>
            </div>

            {/* Top Equipment */}
            <div className="bg-white shadow rounded-2xl p-4">
                <div className="flex justify-between items-center mb-4">
                    <h2 className="text-xl font-bold">Top sprzęt</h2>
                    <div className="space-x-2">
                        <button onClick={() => downloadCsv("top-equipment")} className="bg-blue-500 text-white px-4 py-2 rounded">Export CSV</button>
                        <button onClick={() => downloadPdf("top-equipment")} className="bg-red-500 text-white px-4 py-2 rounded">Export PDF</button>
                    </div>
                </div>

                <div className="overflow-x-auto">
                    <table className="w-full border">
                        <thead className="bg-gray-100">
                        <tr>
                            <th className="p-2 border">Przedmiot</th>
                            <th className="border">Ilość wypożyczeń</th>
                        </tr>
                        </thead>
                        <tbody>
                        {topEquipment.map((t, i) => (
                            <tr key={i} className="border-t hover:bg-gray-50">
                                <td className="p-2 border">{t.equipment}</td>
                                <td className="p-2 border">{t.count}</td>
                            </tr>
                        ))}
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    );
}
