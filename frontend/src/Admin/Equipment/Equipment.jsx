import React, { useEffect, useState } from "react";
import api from "../../api"; 
import EquipmentList from "./EquipmentList";
import EquipmentForm from "./EquipmentForm";

function Equipment() {
    const [equipments, setEquipments] = useState([]);
    const [search, setSearch] = useState("");
    const [view, setView] = useState("list"); 
    const [editData, setEditData] = useState(null);

    useEffect(() => {
        fetchEquipments();
    }, []);

    const fetchEquipments = (query = "") => {
        const url = query
            ? `http://localhost:5000/api/equipment/search?name=${encodeURIComponent(query)}`
            : `http://localhost:5000/api/equipment`;

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
    
    const handleDelete = (id) => {
        if (window.confirm("Czy na pewno chcesz usunąć ten sprzęt?")) {
            api.delete(`http://localhost:5000/api/equipment/${id}`)
                .then(() => fetchEquipments())
                .catch((err) => console.error(err));
        }
    };
    
    const handleSave = (data, isEdit) => {
        if (isEdit) {
            api.put(`http://localhost:5000/api/equipment/${editData.id}`, data)
                .then(() => {
                    fetchEquipments();
                    setView("list");
                })
                .catch((err) => console.error(err));
        } else {
            api.post(`http://localhost:5000/api/equipment`, data)
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