import React, { useState } from 'react';
import axios from './api.js';
import { jwtDecode } from 'jwt-decode';

function LoginPage() {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');

    const handleLogin = async (e) => {
        e.preventDefault();
        setError('');

        try {
            const response = await axios.post('http://localhost:5000/api/account/login', {
                Email: email,
                Password: password
            }, {
                headers: {
                    'Content-Type': 'application/json'
                }
            });

            console.log("Odpowiedź logowania:", response.data);

            const { token, userName } = response.data;
            localStorage.setItem("token", token);
            localStorage.setItem("userName", userName);

            if (!token) {
                setError("Nie udało się pobrać tokena z odpowiedzi.");
                return;
            }

            localStorage.setItem('token', token);
            localStorage.setItem('userName', userName);

            const decodedToken = jwtDecode(token);
            const role = decodedToken["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];

            if (role === "User") {
                window.location.href = "/user-dashboard";
            } else if (role === "Admin") {
                window.location.href = "/admin-dashboard";
            } else if (role === "Manager") {
                window.location.href = "/manager-dashboard";
            }
            else {
                setError("Nieznana rola użytkownika.");
            }

        } catch (err) {
            console.error("Błąd:", err);
            if (err.response?.data?.message) {
                setError(err.response.data.message);
            } else if (!err.response) {
                setError("Brak połączenia z serwerem.");
            } else {
                setError("Wystąpił nieznany błąd.");
            }
        }
    };

    const handleRegisterRedirect = () => {
        // Przekieruj do rejestracji (np. zmień widok lub użyj React Router)
        window.location.href = '/register';
    };

    return (
        <div 
            className="max-w-md mx-auto mt-24 p-6 border border-gray-300 rounded-lg shadow-md text-center font-sans select-none focus:outline-none">
            <img
                src="https://mentorme-programme.eu/wp-content/uploads/2021/02/Spoleczna-Akademia-Nauk.png"
                alt="Logo"
                className="w-40 h-24 mx-auto mb-4 "
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
                    className="bg-blue-600 text-white py-2 rounded hover:bg-blue-700 transition-colors cursor-pointer font-semibold disabled:opacity-50 disabled:cursor-not-allowed"
                >
                    Zaloguj się
                </button>
            </form>
            {error && <p className="text-red-600 mt-3">{error}</p>}
            <p className="mt-5">
                Nie masz konta?{' '}
                <button onClick={handleRegisterRedirect} className="text-blue-600 underline hover:text-blue-800 cursor-pointer">
                    Zarejestruj się
                </button>
            </p>
        </div>
    );
}

export default LoginPage;
