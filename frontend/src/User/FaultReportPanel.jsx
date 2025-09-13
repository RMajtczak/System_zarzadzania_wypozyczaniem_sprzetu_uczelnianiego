import React, { useState } from 'react';
import api from '../api.js';

function FaultReportPanel() {
    const [equipmentName, setEquipmentName] = useState('');
    const [userName, setUserName]= useState('');
    const [description, setDescription] = useState('');
    const [error, setError] = useState(null); // ustawiamy null, bo to może być obiekt lub string
    const [success, setSuccess] = useState('');

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError(null);
        setSuccess('');
        
        

        try {
            await api.post('https://localhost:5001/api/faultreports', {
                equipmentName,
                userName,
                description,
            });
            setSuccess('Usterka została zgłoszona pomyślnie.');
            setEquipmentName('');
            setUserName('');
            setDescription('');
        } catch (err) {
            if (err.response?.data) {
                const data = err.response.data;

                if (data.errors) {
                    const errorsArray = [];
                    for (const key in data.errors) {
                        if (Object.hasOwnProperty.call(data.errors, key)) {
                            const messages = data.errors[key];
                            messages.forEach((msg) => {
                                errorsArray.push(`${key}: ${msg}`);
                            });
                        }
                    }
                    setError(`${data.title}\n${errorsArray.join('\n')}`);
                } else if (data.message) {
                    setError(data.message);
                } else if (data.title) {
                    setError(data.title);
                } else {
                    setError("Wystąpił błąd");
                }
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
                    placeholder="Nazwa użtkownika"
                    value={userName}
                    required
                    onChange={(e) => setUserName(e.target.value)}
                    className="p-3 border border-gray-300 rounded"
                />
                <input
                    type="text"
                    placeholder="Nazwa sprzętu"
                    value={equipmentName}
                    required
                    onChange={(e) => setEquipmentName(e.target.value)}
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

            {error && (
                <pre className="text-red-600 mt-4 whitespace-pre-wrap">{error}</pre>
            )}

            {success && <p className="text-green-600 mt-4">{success}</p>}
        </div>
    );
}

export default FaultReportPanel;
