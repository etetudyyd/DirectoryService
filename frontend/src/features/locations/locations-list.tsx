'use client';

import type { Location} from '@/entities/locations/types'
import { Input } from '@/shared/components/ui/input';
import LocationCard from '@/features/locations/location-card';
import { locationsApi } from '@/entities/locations/api';
import { useState, useEffect } from 'react';
import { Spinner } from '@/shared/components/ui/spinner';

const PAGE_SIZE = 10;

export default function LocationsList() {
const [page, setPage] = useState(1);
const [locations, setLocations] = useState<Location[]>([]);
const [isLoading, setIsLoading] = useState(false);
const [error, setError] = useState<string | null>(null);
const [isEmpty, setIsEmpty] = useState(false);

useEffect(() => {
    setIsLoading(true);
locationsApi
    .getLocations({})
    .then((locations) => {
        setLocations(locations);
        setIsEmpty(locations.length === 0);
    })
    .finally(() => setIsLoading(false))
    .catch((error) => setError(error.message));
}, [page]);


    console.log(locations);

    if (isLoading) {
        return <div className="flex justify-center min-h-screen"><Spinner/></div>;
    }

    if (error) {
        return <div>Error: {error}</div>;
    }

    if (isEmpty) {
        return <div className="text-center mt-20 text-gray-400">No locations found.</div>;
    }



    return (
        <main className="max-w-6xl mx-auto p-6">
            <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-4 mb-6">
                <div>
                <h1 className="text-2xl font-semibold text-white">Локации</h1>
                <p className="text-sm text-slate-400 mt-1">
                    Список всех локаций организации
                </p>
                </div>

                <div className="flex items-center gap-3">
                <Input
                    placeholder="Поиск по названию или адресу"
                    className="min-w-65 bg-slate-800/50"
                />
                </div>
            </div>

            <section className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
                {locations.map((location) => (
                    <LocationCard key={location.id} location={location} />
                ))}
            </section>
        </main>
    )
}