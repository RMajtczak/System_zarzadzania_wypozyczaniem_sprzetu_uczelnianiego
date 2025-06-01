import React, { useState } from 'react';
import axios from './api.js';

function LoginPage() {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');

    const handleLogin = async (e) => {
        e.preventDefault();
        setError('');

        try {
            const response = await axios.post('https://localhost:5001/api/account/login', {
                email,
                password
            });

            const token = response.data.token;
            localStorage.setItem('token', token);
            alert('Zalogowano pomyślnie!');
            // przekierowanie, np. window.location.href = '/dashboard';
        } catch (err) {
            if (err.response?.data?.message) {
                setError(err.response.data.message);
            } else if (!err.response) {
                setError("Brak połączenia z serwerem");
            } else {
                setError("Wystąpił nieznany błąd");
            }
        }
    };

    const handleRegisterRedirect = () => {
        // Przekieruj do rejestracji (np. zmień widok lub użyj React Router)
        window.location.href = '/register';
    };

    return (
        <div className="max-w-md mx-auto mt-24 p-6 border border-gray-300 rounded-lg shadow-md text-center font-sans">
            <img
                src="https://mentorme-programme.eu/wp-content/uploads/2021/02/Spoleczna-Akademia-Nauk.png"
                alt="Logo"
                className="w-40 h-24 mx-auto mb-4"
            />
            <h2 className="text-2xl font-semibold mb-6 ">Logowanie</h2>
            <form onSubmit={handleLogin} className="flex flex-col gap-4">
                <input
                    type="email"
                    placeholder="Email"
                    value={email}
                    required
                    onChange={(e) => setEmail(e.target.value)}
                    className="p-3 border border-gray-300 rounded focus:outline-none focus:ring-2 focus:ring-blue-400"
                />
                <input
                    type="password"
                    placeholder="Hasło"
                    value={password}
                    required
                    onChange={(e) => setPassword(e.target.value)}
                    className="p-3 border border-gray-300 rounded focus:outline-none focus:ring-2 focus:ring-blue-400"
                />
                <button
                    type="submit"
                    className="bg-blue-600 text-white py-2 rounded hover:bg-blue-700 transition-colors"
                >
                    Zaloguj się
                </button>
            </form>
            {error && <p className="text-red-600 mt-3">{error}</p>}
            <p className="mt-5">
                Nie masz konta?{' '}
                <button onClick={handleRegisterRedirect} className="text-blue-600 underline hover:text-blue-800">
                    Zarejestruj się
                </button>
            </p>
        </div>
    );
}

export default LoginPage;
