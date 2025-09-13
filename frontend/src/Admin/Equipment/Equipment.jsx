import React, { useEffect, useState } from "react";
import api from "../../api"; 
import EquipmentList from "./EquipmentList";
import EquipmentForm from "./EquipmentForm";

function Equipment() {
    const [equipments, setEquipments] = useState([]);
    const [search, setSearch] = useState("");
    const [view, setView] = useState("list"); // list | add | edit
    const [editData, setEditData] = useState(null);

    useEffect(() => {
        fetchEquipments();
    }, []);

    // Użycie 'api' i ścieżek względnych
    const fetchEquipments = (query = "") => {
        const url = query ? `/equipment/search?name=${encodeURIComponent(query)}` : `/equipment`;

        api.get(url)
            .then((res) => {
                setEquipments(res.data);
            })
            .catch((err) => {
                console.error(err);
                setEquipments([]);
            });
    };

    const handleSearch = (e) => {
        e.preventDefault();
        fetchEquipments(search);
    };

    const handleAdd = () => {
        setEditData(null);
        setView("add");
    };

    const handleEdit = (id) => {
        const eq = equipments.find((e) => e.id === id);
        setEditData(eq);
        setView("edit");
    };

    // Użycie 'api' i ścieżek względnych
    const handleDelete = (id) => {
        if (window.confirm("Czy na pewno chcesz usunąć ten sprzęt?")) {
            api.delete(`/equipment/${id}`)
                .then(() => fetchEquipments())
                .catch((err) => console.error(err));
        }
    };

    // Użycie 'api' i ścieżek względnych
    const handleSave = (data, isEdit) => {
        if (isEdit) {
            api.put(`/equipment/${editData.id}`, data)
                .then(() => {
                    fetchEquipments();
                    setView("list");
                })
                .catch((err) => console.error(err));
        } else {
            api.post(`/equipment`, data)
                .then(() => {
                    fetchEquipments();
                    setView("list");
                })
                .catch((err) => console.error(err));
        }
    };

    if (view === "list") {
        return (
            <EquipmentList
                equipments={equipments}
                search={search}
                setSearch={setSearch}
                onSearch={handleSearch}
                onAdd={handleAdd}
                onEdit={handleEdit}
                onDelete={handleDelete}
            />
        );
    }

    if (view === "add" || view === "edit") {
        return (
            <EquipmentForm
                initialData={editData}
                onSave={(data) => handleSave(data, view === "edit")}
                onCancel={() => setView("list")}
            />
        );
    }

    return null;
}

export default Equipment;