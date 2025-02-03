import React, { useState, useEffect } from 'react';
import * as signalR from '@microsoft/signalr';

// Define the structure of the expected query result.
interface QueryResult {
    id: number;
    someProperty: string;
}

const QueryComponent: React.FC = () => {
    const [jobId, setJobId] = useState<string>('');
    const [result, setResult] = useState<QueryResult[] | null>(null);
    const [error, setError] = useState<string>('');
    const [connection, setConnection] = useState<signalR.HubConnection | null>(null);

    // Create and start the SignalR connection.
    useEffect(() => {
        // IMPORTANT: Adjust the URL to match your backend SignalR hub address.
        const newConnection = new signalR.HubConnectionBuilder()
            .withUrl('http://localhost:7256/queryHub') // Change if needed.
            .withAutomaticReconnect()
            .build();

        newConnection
            .start()
            .then(() => {
                console.log('SignalR connection established.');
                // If a jobId already exists, join its group.
                if (jobId) {
                    newConnection.invoke('JoinJobGroup', jobId)
                        .catch(err => console.error('Error joining group:', err));
                }
            })
            .catch(err => console.error('SignalR connection error:', err));

        setConnection(newConnection);

        // Cleanup on unmount.
        return () => {
            newConnection.stop();
        };
    }, [jobId]);

    // Set up SignalR event listeners.
    useEffect(() => {
        if (connection) {
            connection.on('QueryCompleted', (receivedJobId: string, queryResult: QueryResult[]) => {
                if (receivedJobId === jobId) {
                    setResult(queryResult);
                }
            });

            connection.on('QueryFailed', (receivedJobId: string, errorMessage: string) => {
                if (receivedJobId === jobId) {
                    setError(errorMessage);
                }
            });
        }
    }, [connection, jobId]);

    // Start the query job by calling the backend API.
    const startQuery = async () => {
        // Reset any previous result or error.
        setResult(null);
        setError('');
        try {
            const response = await fetch('https://localhost:7256/api/query/start', {  // Note: This endpoint may be proxied.
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ queryParameter: 'someValue' })
            });

            if (response.status === 202) {
                const data = await response.json();
                setJobId(data.jobId);
                // Once we have a jobId, join its SignalR group.
                if (connection) {
                    connection.invoke('JoinJobGroup', data.jobId)
                        .catch(err => console.error('Error joining group after jobId update:', err));
                }
            } else {
                console.error('Error starting query:', response.statusText);
            }
        } catch (err: any) {
            console.error('Error starting query:', err);
        }
    };

    return (
        <div>
            <h2>Long-Running Query</h2>
            <button onClick={startQuery}>Start Query</button>
            {jobId && <p>Job ID: {jobId}</p>}
            {error && <p style={{ color: 'red' }}>Error: {error}</p>}
            {result && (
                <div>
                    <h3>Query Result:</h3>
                    <pre>{JSON.stringify(result, null, 2)}</pre>
                </div>
            )}
        </div>
    );
};

export default QueryComponent;