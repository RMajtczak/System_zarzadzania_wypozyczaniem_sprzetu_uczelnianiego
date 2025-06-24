import React, { useState } from 'react';
import axios from './api.js';

function FaultReportPanel() {
    const [equipmentId, setEquipmentId] = useState('');
    const [description, setDescription] = useState('');
    const [error, setError] = useState('');
    const [success, setSuccess] = useState('');

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');
        setSuccess('');

        try {
            await axios.post('https://localhost:5001/api/faults', {
                equipmentId,
                description,
            });
            setSuccess('Usterka została zgłoszona pomyślnie.');
            setEquipmentId('');
            setDescription('');
        } catch (err) {
            if (err.response?.data) {
                setError(err.response.data);
            } else {
                setError('Wystąpił błąd podczas zgłaszania usterki.');
            }
        }
    };

    return (
        <div className="max-w-md mx-auto py-8 px-4">
            <h2 className="text-2xl font-semibold mb-6">Zgłoś usterkę</h2>
            <form onSubmit={handleSubmit} className="flex flex-col gap-4">
                <input
                    type="text"
                    placeholder="ID sprzętu"
                    value={equipmentId}
                    required
                    onChange={(e) => setEquipmentId(e.target.value)}
                    className="p-3 border border-gray-300 rounded"
                />
                <textarea
                    placeholder="Opis usterki"
                    value={description}
                    required
                    onChange={(e) => setDescription(e.target.value)}
                    className="p-3 border border-gray-300 rounded"
                />
                <button
                    type="submit"
                    className="bg-red-600 text-white p-3 rounded hover:bg-red-700 transition"
                >
                    Zgłoś usterkę
                </button>
            </form>
            {error && <p className="text-red-600 mt-4">{error}</p>}
            {success && <p className="text-green-600 mt-4">{success}</p>}
        </div>
    );
}

export default FaultReportPanel;