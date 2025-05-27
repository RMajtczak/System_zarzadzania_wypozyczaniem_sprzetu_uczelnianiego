import React, { useEffect, useState } from 'react';
import axios from 'axios';

function App() {
    const [equipment, setEquipment] = useState([]);

    useEffect(() => {
        axios.get('http://localhost:5000/api/equipment')
            .then(response => {
                console.log('Sprzęt:', response.data);
                setEquipment(response.data);
            })
            .catch(error => {
                console.error('Błąd:', error.message);
            });
    }, []);


    return (
        <div style={{ padding: '20px' }}>
            <h1>Lista Sprzętu</h1>
            <ul>
                {equipment.map((item) => (
                    <li key={item.id}>
                        <strong>{item.name}</strong> — {item.type}, status: {item.status}
                    </li>
                ))}
            </ul>
        </div>
    );
}

export default App;
