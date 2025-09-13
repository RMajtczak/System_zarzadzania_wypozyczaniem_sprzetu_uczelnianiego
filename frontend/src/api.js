// Plik: frontend/src/api.js
import axios from 'axios';

const api = axios.create({
    // 1. Poprawiony baseURL na właściwy adres i port API
    baseURL: 'https://localhost:5001/api',
    withCredentials: true,
});

// 2. Dodany interceptor do dołączania tokenu
api.interceptors.request.use(
    (config) => {
        const token = localStorage.getItem('token');
        if (token) {
            // Dołącz nagłówek Authorization do każdego zapytania
            config.headers['Authorization'] = `Bearer ${token}`;
        }
        return config;
    },
    (error) => {
        return Promise.reject(error);
    }
);

export default api;