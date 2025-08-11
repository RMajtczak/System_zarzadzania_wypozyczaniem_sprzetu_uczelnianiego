import React from "react";

function EquipmentList({ equipments, search, setSearch, onSearch, onAdd, onEdit, onDelete }) {
    return (
        <div>
            <form onSubmit={onSearch} className="flex gap-2 mb-4">
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
                <button type="button" onClick={onAdd} className="bg-green-500 text-white px-4 py-2 rounded">
                    Dodaj
                </button>
            </form>

            <table className="w-full border">
                <thead>
                <tr className="bg-gray-200">
                    <th className="border px-4 py-2">ID</th>
                    <th className="border px-4 py-2">Nazwa</th>
                    <th className="border px-4 py-2">Status</th>
                    <th className="border px-4 py-2">Akcje</th>
                </tr>
                </thead>
                <tbody>
                {equipments.length > 0 ? (
                    equipments.map((eq) => (
                        <tr key={eq.id}>
                            <td className="border px-4 py-2">{eq.id}</td>
                            <td className="border px-4 py-2">{eq.name}</td>
                            <td className="border px-4 py-2">{eq.status}</td>
                            <td className="border px-4 py-2 text-center">
                                <div className="flex justify-center gap-2">
                                    <button
                                        className="bg-yellow-500 text-white px-2 py-1 rounded"
                                        onClick={() => onEdit(eq.id)}
                                    >
                                        Edytuj
                                    </button>
                                    <button
                                        className="bg-red-500 text-white px-2 py-1 rounded"
                                        onClick={() => onDelete(eq.id)}
                                    >
                                        Usuń
                                    </button>
                                </div>
                            </td>
                        </tr>
                    ))
                ) : (
                    <tr>
                        <td colSpan="4" className="text-center p-4">
                            Brak sprzętu
                        </td>
                    </tr>
                )}
                </tbody>
            </table>
        </div>
    );
}

export default EquipmentList;
