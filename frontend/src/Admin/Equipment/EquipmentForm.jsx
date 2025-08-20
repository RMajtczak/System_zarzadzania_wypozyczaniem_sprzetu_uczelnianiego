import React, { useState, useEffect } from "react";

const statusOptions = [
    { label: "Dostępny", value: 0 },
    { label: "Zarezerwowany", value: 1 },
    { label: "Wypożyczony", value: 2 },
    { label: "Naprawa", value: 3 },
    { label: "Uszkodzony", value: 4 },
];

function EquipmentForm({ initialData, onSave, onCancel }) {
    const [name, setName] = useState("");
    const [type, setType] = useState("");
    const [serialNumber, setSerialNumber] = useState("");
    const [specification, setSpecification] = useState("");
    const [location, setLocation] = useState("");
    const [status, setStatus] = useState(0);

    useEffect(() => {
        
        if (initialData) {
            setName(initialData.name || "");
            setType(initialData.type || "");
            setSerialNumber(initialData.serialNumber || "");
            setSpecification(initialData.specification || "");
            setLocation(initialData.location || "");
            const statusIndex = statusOptions.findIndex(
                (opt) => opt.label === initialData.status
            );
            setStatus(statusIndex >= 0 ? statusIndex : 0);
        }
    }, [initialData]);

    const handleSubmit = (e) => {
        e.preventDefault();
        onSave({
            name,
            type,
            serialNumber,
            specification,
            location,
            status,
        });
    };

    return (
        <div className="max-w-md mx-auto bg-white p-6 rounded shadow">
            <h2 className="text-xl font-semibold mb-4">
                {initialData ? "Edytuj sprzęt" : "Dodaj sprzęt"}
            </h2>
            <form onSubmit={handleSubmit} className="space-y-4">
                <input
                    type="text"
                    placeholder="Nazwa"
                    value={name}
                    onChange={(e) => setName(e.target.value)}
                    required
                    className="w-full border px-3 py-2 rounded"
                />
                <input
                    type="text"
                    placeholder="Typ"
                    value={type}
                    onChange={(e) => setType(e.target.value)}
                    required
                    className="w-full border px-3 py-2 rounded"
                />
                <input
                    type="text"
                    placeholder="Numer seryjny"
                    value={serialNumber}
                    onChange={(e) => setSerialNumber(e.target.value)}
                    required
                    className="w-full border px-3 py-2 rounded"
                />
                <input
                    type="text"
                    placeholder="Specyfikacja"
                    value={specification}
                    onChange={(e) => setSpecification(e.target.value)}
                    required
                    className="w-full border px-3 py-2 rounded"
                />
                <input
                    type="text"
                    placeholder="Lokalizacja"
                    value={location}
                    onChange={(e) => setLocation(e.target.value)}
                    required
                    className="w-full border px-3 py-2 rounded"
                />
                <select
                    value={status}
                    onChange={(e) => setStatus(parseInt(e.target.value))}
                    className="w-full border px-3 py-2 rounded"
                >
                    {statusOptions.map(({ label, value }) => (
                        <option key={value} value={value}>
                            {label}
                        </option>
                    ))}
                </select>
                <div className="flex justify-between">
                    <button
                        type="button"
                        onClick={onCancel}
                        className="bg-gray-400 text-white px-4 py-2 rounded"
                    >
                        Anuluj
                    </button>
                    <button
                        type="submit"
                        className="bg-blue-600 text-white px-4 py-2 rounded"
                    >
                        Zapisz
                    </button>
                </div>
            </form>
        </div>
    );
}

export default EquipmentForm;
