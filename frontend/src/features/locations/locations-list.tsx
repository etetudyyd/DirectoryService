'use client';
import { useMutation, useQuery } from '@tanstack/react-query';
import { Input } from '@/shared/components/ui/input';
import LocationCard from '@/features/locations/location-card';
import { locationsApi } from '@/entities/locations/api';
import { useState } from 'react';
import { Spinner } from '@/shared/components/ui/spinner';
import LocationsPagination from './location-pagination';
import { Button } from '@/shared/components/ui/button';
import { queryClient } from '@/shared/api/query-client';
import { CreateLocationDialog } from './create-location-dialog';
import { useLocationsList } from './model/use-locations-list';

export default function LocationsList() {

const [open, setOpen] = useState(false);

const [page, setPage] = useState(1);

const {locations, totalPages, totalItems, isLoading, error} = useLocationsList({ page });

const {
    mutate: createLocation,
     isPending,
    error: createError} = useMutation({
    mutationFn: () => 
        locationsApi.createLocation({ 
            name: "location1",
            address: {
                country: "Country 1",
                region: "Region 1",
                city: "City 1",
                street: "Street 1",
                house: "House 1",
                postalCode: "123456",
                apartment: "1A" 
            },
            timeZone: "timezone1",
            departmentsIds: ["c37d25da-3d1c-4973-9b8a-b7611ebce5bb"]}),
    onSettled: () => queryClient.invalidateQueries({ queryKey: ["locations"] }),
    onSuccess: () => {
        console.log("Location created successfully");
    },
    onError: (error) => {
        console.error("Error creating location:", error);
    },
});
    
    console.log(locations);

    if (isLoading) {
        return <div className="flex justify-center min-h-screen"><Spinner/></div>;
    }

    if (error) {
        return <div>Error: {error.message}</div>;
    }

    if (!locations || totalItems === 0) {
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
                <Button onClick={() => setOpen(true)} disabled={isPending} variant="default">Добавить локацию</Button>
                {createError && <div className="text-red-500 text-sm">{createError.message}</div>}
                </div>
            </div>

            <section className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
                {locations.map((location) => (
                    <LocationCard key={location.id} location={location} />
                ))}
            </section>

            {totalPages &&
            (<LocationsPagination 
                page={page} 
                totalPages={totalPages} 
                onPageChange={(p) => setPage(p)} />)}
            

            <CreateLocationDialog 
                open={open} 
                onOpenChange={setOpen}/>
        </main>

        
    )
}