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
                <button type="button" onClick={onAdd} className="bg-blue-500 text-white px-4 py-2 rounded">
                    Dodaj
                </button>
            </form>

            <table className="min-w-full divide-y divide-gray-200">
                <thead className="bg-gray-200">
                <tr>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Nazwa
                        sprzętu
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Numer
                        seryjny
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Status</th>
                    <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Akcje</th>
                </tr>
                </thead>
                <tbody className="bg-white divide-y divide-gray-200">
                {equipments.length > 0 ? (
                    equipments.map((eq) => (
                        <tr key={eq.id} className="hover:bg-gray-50">
                            <td className="px-6 py-4 whitespace-nowrap">
                                <div className="text-sm font-medium text-gray-900">{eq.name}</div>
                                <div className="text-sm text-gray-500">{eq.type}</div>
                            </td>
                            <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">{eq.serialNumber}</td>
                            <td className="px-6 py-4 whitespace-nowrap">
            <span
                className={`px-2 inline-flex text-xs leading-5 font-semibold rounded-full ${
                    eq.status === 'Dostępny'
                        ? 'bg-green-100 text-green-800'
                        : eq.status === 'Zarezerwowany'
                            ? 'bg-yellow-100 text-yellow-800'
                            : eq.status === 'Wypożyczony'
                                ? 'bg-blue-100 text-blue-800'
                                : eq.status === 'Naprawa'
                                    ? 'bg-orange-100 text-orange-800'
                                    : 'bg-red-100 text-red-800'
                }`}
            >
              {eq.status}
            </span>
                            </td>
                            <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium space-x-4">
                                <button
                                    onClick={() => onEdit(eq.id)}
                                    className="text-indigo-600 hover:text-indigo-900"
                                >
                                    Edytuj
                                </button>
                                <button
                                    onClick={() => onDelete(eq.id)}
                                    className="text-red-600 hover:text-red-900"
                                >
                                    Usuń
                                </button>
                            </td>
                        </tr>
                    ))
                ) : (
                    <tr>
                        <td colSpan="4" className="px-6 py-4 text-center text-gray-500">
                            Brak sprzętu do wyświetlenia
                        </td>
                    </tr>
                )}
                </tbody>
            </table>
        </div>
    );
}

export default EquipmentList;
