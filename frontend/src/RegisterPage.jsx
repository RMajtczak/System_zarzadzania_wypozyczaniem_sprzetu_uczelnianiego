import React, { useState } from 'react';
import axios from './api.js'; 

function RegisterPage() {
    const [firstName, setFirstName] = useState('');
    const [lastName, setLastName] = useState('');
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');
    const [error, setError] = useState('');
    const [success, setSuccess] = useState('');

    const handleRegister = async (e) => {
        e.preventDefault();
        setError('');
        setSuccess('');

        if (password !== confirmPassword) {
            setError("Hasła nie są takie same");
            return;
        }

        try {
            await axios.post('http://localhost:5000/api/account/register', {
                Email: email,
                Password: password,
                ConfirmPassword: confirmPassword,
                FirstName: firstName,
                LastName: lastName,
                RoleId: 1
            });
            setSuccess("Rejestracja przebiegła pomyślnie! Możesz się teraz zalogować.");
            setFirstName('');
            setLastName('');
            setEmail('');
            setPassword('');
            setConfirmPassword('');
        } catch (err) {
            if (err.response?.data?.errors) {
                
                const validationErrors = err.response.data.errors;
                const messages = Object.values(validationErrors).flat(); 
                setError(messages.join('\n')); 
            } else if (!err.response) {
                setError("Brak połączenia z serwerem");
            } else {
                setError("Wystąpił nieznany błąd");
            }
        }
    };

    const handleLoginRedirect = () => {
        window.location.href = '/login';
    };

    return (
        <div className="max-w-md mx-auto mt-20 p-6 border border-gray-300 rounded-lg shadow-md font-sans text-center">
            <img
                src="https://mentorme-programme.eu/wp-content/uploads/2021/02/Spoleczna-Akademia-Nauk.png"
                alt="Logo"
                className="w-40 h-24 mx-auto"
            />
            <h2 className="text-2xl font-semibold mb-6">Rejestracja</h2>
            <form onSubmit={handleRegister} className="flex flex-col gap-4">
                <input
                    type="text"
                    placeholder="Imię"
                    value={firstName}
                    required
                    onChange={(e) => setFirstName(e.target.value)}
                    className="p-3 border border-gray-300 rounded"
                />
                <input
                    type="text"
                    placeholder="Nazwisko"
                    value={lastName}
                    required
                    onChange={(e) => setLastName(e.target.value)}
                    className="p-3 border border-gray-300 rounded"
                />
                <input
                    type="email"
                    placeholder="Email"
                    value={email}
                    required
                    onChange={(e) => setEmail(e.target.value)}
                    className="p-3 border border-gray-300 rounded"
                />
                <input
                    type="password"
                    placeholder="Hasło"
                    value={password}
                    required
                    onChange={(e) => setPassword(e.target.value)}
                    className="p-3 border border-gray-300 rounded"
                />
                <input
                    type="password"
                    placeholder="Powtórz hasło"
                    value={confirmPassword}
                    required
                    onChange={(e) => setConfirmPassword(e.target.value)}
                    className="p-3 border border-gray-300 rounded"
                />
                <button
                    type="submit"
                    className="bg-blue-600 text-white py-2 rounded hover:bg-blue-700 transition-colors cursor-pointer font-semibold disabled:opacity-50 disabled:cursor-not-allowed"
                >
                    Zarejestruj się
                </button>
            </form>

            {error && <p className="text-red-600 mt-4">{error}</p>}
            {success && <p className="text-green-600 mt-4">{success}</p>}

            <p className="mt-6">
                Masz już konto?{' '}
                <button
                    onClick={handleLoginRedirect}
                    className="text-blue-600 underline hover:text-blue-800 cursor-pointer"
                >
                    Zaloguj się
                </button>
            </p>
        </div>
    );
}

export default RegisterPage;
