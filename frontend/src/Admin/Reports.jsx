import React, { useEffect, useState } from "react";

function ReportsPanel() {
    const [report, setReport] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
        fetch("https://localhost:5001/api/reports/equipment-stats")
            .then(res => {
                if (!res.ok) throw new Error(`HTTP error! status: ${res.status}`);
                return res.json();
            })
            .then(data => {
                setReport(data);
                setLoading(false);
            })
            .catch(err => {
                console.error("Błąd pobierania raportu:", err);
                setError(err.message);
                setLoading(false);
            });
    }, []);

    const handleDownload = (type) => {
        const url = type === "pdf"
            ? "https://localhost:5001/api/reports/equipment-report/pdf"
            : "https://localhost:5001/api/reports/equipment-report/csv";
        window.open(url, "_blank");
    };

    if (loading) return <p>Ładowanie raportu...</p>;
    if (error) return <p className="text-red-600">Błąd: {error}</p>;

    return (
        <div className="max-w-6xl mx-auto py-8 px-4">
            <div className="bg-white shadow rounded-lg p-6">
                <h2 className="text-xl font-bold mb-4">Statystyki sprzętu</h2>

                <table className="min-w-full divide-y divide-gray-200 border">
                    <thead className="bg-gray-200">
                    <tr>
                        <th className="px-4 py-2 text-left">Total</th>
                        <th className="px-4 py-2 text-left">Dostępny</th>
                        <th className="px-4 py-2 text-left">Zarezerwowany</th>
                        <th className="px-4 py-2 text-left">Wypożyczony</th>
                        <th className="px-4 py-2 text-left">Naprawa</th>
                        <th className="px-4 py-2 text-left">Uszkodzony</th>
                    </tr>
                    </thead>
                    <tbody className="bg-white divide-y divide-gray-200">
                    <tr>
                        <td className="px-4 py-2">{report.total}</td>
                        <td className="px-4 py-2">{report.dostępny}</td>
                        <td className="px-4 py-2">{report.zarezerwowany}</td>
                        <td className="px-4 py-2">{report.wypożyczony}</td>
                        <td className="px-4 py-2">{report.naprawa}</td>
                        <td className="px-4 py-2">{report.uszkodzony}</td>
                    </tr>
                    </tbody>
                </table>

                <div className="flex space-x-4 mt-6">
                    <button
                        className="bg-green-600 text-white px-4 py-2 rounded hover:bg-green-700"
                        onClick={() => handleDownload("pdf")}
                    >
                        Pobierz PDF
                    </button>
                    <button
                        className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700"
                        onClick={() => handleDownload("csv")}
                    >
                        Pobierz CSV
                    </button>
                </div>
            </div>
        </div>
    );
}

export default ReportsPanel;
