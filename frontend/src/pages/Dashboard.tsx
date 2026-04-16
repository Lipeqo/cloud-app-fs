import React, { useEffect, useState } from 'react';
import api from '../services/api';

interface CloudTask {
  id: number;
  name: string;
  isCompleted: boolean;
}

const pageStyle: React.CSSProperties = {
  padding: '32px 20px',
  textAlign: 'center',
  fontFamily: 'Arial, sans-serif',
  minHeight: '100vh',
  background: 'linear-gradient(180deg, #f4f8ff 0%, #ffffff 100%)',
  color: '#1f2937',
};

const Dashboard = () => {
  const [items, setItems] = useState<CloudTask[]>([]);
  const [error, setError] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [newTaskName, setNewTaskName] = useState('');

  const fetchTasks = async () => {
    setIsLoading(true);
    setError('');

    try {
      const response = await api.get<CloudTask[]>('/tasks');
      setItems(response.data);
    } catch (err) {
      console.error('Szczegóły błędu:', err);
      setError('Błąd połączenia z API. Sprawdź, czy backend działa oraz czy VITE_API_URL wskazuje poprawny adres.');
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    void fetchTasks();
  }, []);

  const handleAddTask = async (e: React.FormEvent) => {
    e.preventDefault();

    const trimmedName = newTaskName.trim();
    if (!trimmedName) {
      setError('Nazwa zadania nie może być pusta.');
      return;
    }

    setError('');
    setIsLoading(true);

    try {
      await api.post('/tasks', { name: trimmedName });
      setNewTaskName('');
      await fetchTasks();
    } catch (err) {
      console.error('Błąd podczas dodawania zadania:', err);
      setError('Nie udało się dodać zadania. Spróbuj ponownie.');
    } finally {
      setIsLoading(false);
    }
  };

  const toggleCompletion = async (task: CloudTask) => {
    setError('');
    setIsLoading(true);

    try {
      await api.put(`/tasks/${task.id}`, {
        name: task.name,
        isCompleted: !task.isCompleted,
      });
      await fetchTasks();
    } catch (err) {
      console.error('Błąd podczas aktualizacji zadania:', err);
      setError('Nie udało się zaktualizować zadania. Spróbuj ponownie.');
    } finally {
      setIsLoading(false);
    }
  };

  const deleteTask = async (id: number) => {
    setError('');
    setIsLoading(true);

    try {
      await api.delete(`/tasks/${id}`);
      await fetchTasks();
    } catch (err) {
      console.error('Błąd podczas usuwania zadania:', err);
      setError('Nie udało się usunąć zadania. Spróbuj ponownie.');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div style={pageStyle}>
      {/* 🔥 ZMIANA DO 8.3 */}
      <h1 style={{ marginBottom: '8px', color: '#dc2626' }}>
        ☁️ Cloud App Dashboard - CI/CD OK
      </h1>

      <p style={{ color: 'green', fontWeight: 'bold', marginBottom: '20px' }}>
        Wdrożenie automatyczne działa poprawnie.
      </p>

      {error && (
        <div
          style={{
            background: '#fff3cd',
            color: '#856404',
            padding: '12px',
            borderRadius: '8px',
            margin: '20px auto',
            maxWidth: '520px',
          }}
        >
          {error}
        </div>
      )}

      <form onSubmit={handleAddTask} style={{ marginBottom: '24px' }}>
        <input
          type="text"
          placeholder="Wpisz nowe zadanie..."
          value={newTaskName}
          onChange={(e) => setNewTaskName(e.target.value)}
          style={{
            padding: '12px',
            width: '280px',
            borderRadius: '8px',
            border: '1px solid #cbd5e1',
          }}
          disabled={isLoading}
        />
        <button
          type="submit"
          disabled={isLoading || !newTaskName.trim()}
          style={{
            marginLeft: '10px',
            padding: '12px 20px',
            backgroundColor: '#2563eb',
            color: 'white',
            border: 'none',
            borderRadius: '8px',
            cursor: 'pointer',
            opacity: isLoading ? 0.7 : 1,
          }}
        >
          {isLoading ? 'Ładowanie...' : 'Dodaj zadanie'}
        </button>
      </form>

      <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
        {isLoading && items.length === 0 && <p>Ładowanie zadań...</p>}
        {!isLoading && items.length === 0 && !error && <p>Brak zadań. Czas coś zaplanować!</p>}

        <ul style={{ listStyle: 'none', padding: 0, width: '100%', maxWidth: '520px' }}>
          {items.map((item) => (
            <li
              key={item.id}
              style={{
                background: '#ffffff',
                margin: '10px 0',
                padding: '14px 16px',
                borderRadius: '12px',
                borderLeft: item.isCompleted ? '6px solid #16a34a' : '6px solid #64748b',
                textAlign: 'left',
                boxShadow: '0 8px 20px rgba(15, 23, 42, 0.08)',
                display: 'flex',
                justifyContent: 'space-between',
                alignItems: 'center',
                gap: '12px',
              }}
            >
              <span>
                <strong>{item.name}</strong> {item.isCompleted ? '✅' : '⏳'}
              </span>

              <span>
                <button
                  onClick={() => void toggleCompletion(item)}
                  disabled={isLoading}
                  style={{
                    marginRight: '6px',
                    padding: '8px 12px',
                    backgroundColor: item.isCompleted ? '#475569' : '#16a34a',
                    color: '#fff',
                    border: 'none',
                    borderRadius: '6px',
                    cursor: 'pointer',
                  }}
                >
                  {item.isCompleted ? 'Przywróć' : 'Zakończ'}
                </button>
                <button
                  onClick={() => void deleteTask(item.id)}
                  disabled={isLoading}
                  style={{
                    padding: '8px 12px',
                    backgroundColor: '#dc2626',
                    color: '#fff',
                    border: 'none',
                    borderRadius: '6px',
                    cursor: 'pointer',
                  }}
                >
                  Usuń
                </button>
              </span>
            </li>
          ))}
        </ul>
      </div>
    </div>
  );
};

export default Dashboard;