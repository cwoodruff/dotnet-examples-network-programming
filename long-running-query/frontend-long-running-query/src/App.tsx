import React from 'react';
import QueryComponent from './components/QueryComponent';
import './App.css';

const App: React.FC = () => {
    return (
        <div className="App">
            <h1>Vite React SignalR Client</h1>
            <QueryComponent />
        </div>
    );
};

export default App;
