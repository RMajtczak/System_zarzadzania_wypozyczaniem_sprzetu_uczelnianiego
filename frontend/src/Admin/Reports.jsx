import React, { useEffect, useState } from "react";
import { BarChart, Bar, XAxis, YAxis, Tooltip, CartesianGrid, ResponsiveContainer } from "recharts";
import { CSVLink } from "react-csv";
import jsPDF from "jspdf";
import autoTable from "jspdf-autotable";

const BorrowingReport = () => {
    const [report, setReport] = useState([]);
    const [stats, setStats] = useState([]);

    useEffect(() => {
        // Pobranie raportu wypożyczeń
        fetch("https://localhost:5001/api/reports/borrowings")
            .then(res => res.json())
            .then(setReport);

        // Pobranie statystyk sprzętu
        fetch("https://localhost:5001/api/reports/stats/equipment")
            .then(res => res.json())
            .then(setStats);
    }, []);

    const generatePDF = () => {
        const doc = new jsPDF();
        doc.text("Raport wypożyczeń sprzętu", 14, 15);

        const tableColumn = ["Sprzęt", "Użytkownik", "Data rozpoczęcia", "Data zakończenia", "Stan", "Zwrócony"];
        const tableRows = report.map(b => [
            b.EquipmentName,
            b.BorrowerName,
            new Date(b.StartDate).toLocaleDateString(),
            b.EndDate ? new Date(b.EndDate).toLocaleDateString() : "-",
            b.Condition,
            b.IsReturned ? "Tak" : "Nie"
        ]);

        autoTable(doc, {
            head: [tableColumn],
            body: tableRows,
            startY: 20
        });

        doc.save("wypozyczenia.pdf");
    };


    return (
        <div className="p-4 bg-white rounded shadow">
            <h2 className="text-xl font-bold mb-4">Raport wypożyczeń</h2>

            <div className="mb-4 flex gap-2">
                <CSVLink
                    data={report}
                    filename={"wypozyczenia.csv"}
                    className="px-4 py-2 bg-blue-600 text-white rounded"
                >
                    Eksport CSV
                </CSVLink>
                <button
                    onClick={generatePDF}
                    className="px-4 py-2 bg-green-600 text-white rounded"
                >
                    Eksport PDF
                </button>
            </div>

            <div className="mb-6">
                <h3 className="font-bold mb-2">Statystyki sprzętu (ilość wypożyczeń)</h3>
                <ResponsiveContainer width="100%" height={300}>
                    <BarChart data={stats} margin={{ top: 20, right: 30, left: 0, bottom: 5 }}>
                        <CartesianGrid strokeDasharray="3 3" />
                        <XAxis dataKey="EquipmentName" />
                        <YAxis />
                        <Tooltip />
                        <Bar dataKey="Count" fill="#8884d8" />
                    </BarChart>
                </ResponsiveContainer>
            </div>

            <table className="w-full border border-gray-300">
                <thead>
                <tr className="bg-gray-100">
                    <th className="p-2 border">Sprzęt</th>
                    <th className="p-2 border">Użytkownik</th>
                    <th className="p-2 border">Data rozpoczęcia</th>
                    <th className="p-2 border">Data zakończenia</th>
                    <th className="p-2 border">Stan</th>
                    <th className="p-2 border">Zwrócony</th>
                </tr>
                </thead>
                <tbody>
                {report.map((b, idx) => (
                    <tr key={idx}>
                        <td className="p-2 border">{b.EquipmentName}</td>
                        <td className="p-2 border">{b.BorrowerName}</td>
                        <td className="p-2 border">{new Date(b.StartDate).toLocaleDateString()}</td>
                        <td className="p-2 border">{b.EndDate ? new Date(b.EndDate).toLocaleDateString() : "-"}</td>
                        <td className="p-2 border">{b.Condition}</td>
                        <td className="p-2 border">{b.IsReturned ? "Tak" : "Nie"}</td>
                    </tr>
                ))}
                </tbody>
            </table>
        </div>
    );
};

export default BorrowingReport;
