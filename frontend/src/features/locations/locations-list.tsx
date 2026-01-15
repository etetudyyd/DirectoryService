'use client';
import { useMutation, useQuery } from '@tanstack/react-query';
import { Input } from '@/shared/components/ui/input';
import LocationCard from '@/features/locations/location-card';
import { locationsApi } from '@/entities/locations/api';
import { useState } from 'react';
import { Spinner } from '@/shared/components/ui/spinner';
import {
  Pagination,
  PaginationContent,
  PaginationEllipsis,
  PaginationItem,
  PaginationLink,
  PaginationNext,
  PaginationPrevious,
} from '@/shared/components/ui/pagination';
import { Button } from '@/shared/components/ui/button';
import { queryClient } from '@/shared/api/query-client';

const PAGE_SIZE = 3;

export default function LocationsList() {

const [page, setPage] = useState(1);

const {
    data: locations,
    isLoading,
    error
    } = useQuery({
    queryKey: ["locations", {page}],
    queryFn: () => locationsApi.getLocations({ page: page, pageSize: PAGE_SIZE }),
});

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

    if (!locations || locations.totalItems === 0) {
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
                <Button onClick={() => createLocation()} disabled={isPending} variant="default">Добавить локацию</Button>
                {createError && <div className="text-red-500 text-sm">{createError.message}</div>}
                </div>
            </div>

            <section className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
                {locations.items.map((location) => (
                    <LocationCard key={location.id} location={location} />
                ))}
            </section>

            <div className="mt-8">
                <Pagination>
                    <PaginationContent>
                        <PaginationItem>
                            <PaginationPrevious
                                href="#"
                                onClick={(e) => {
                                    e.preventDefault();
                                    if (locations.page > 1) setPage(locations.page - 1);
                                }}
                                className={locations.page === 1 ? 'pointer-events-none opacity-50' : ''}
                            />
                        </PaginationItem>

                        {Array.from({ length: locations.totalPages }, (_, i) => i + 1).map((pageNum) => {
                            const isCurrentPage = pageNum === locations.page;
                            const isNearCurrent = Math.abs(pageNum - locations.page) <= 1;
                            const isFirst = pageNum === 1;
                            const isLast = pageNum === locations.totalPages;
                            const shouldShow = isFirst || isLast || isNearCurrent;

                            if (!shouldShow && pageNum === 2) {
                                return (
                                    <PaginationItem key="ellipsis-start">
                                        <PaginationEllipsis />
                                    </PaginationItem>
                                );
                            }

                            if (!shouldShow && pageNum === locations.totalPages - 1) {
                                return (
                                    <PaginationItem key="ellipsis-end">
                                        <PaginationEllipsis />
                                    </PaginationItem>
                                );
                            }

                            if (!shouldShow) return null;

                            return (
                                <PaginationItem key={pageNum}>
                                    <PaginationLink
                                        href="#"
                                        isActive={isCurrentPage}
                                        onClick={(e) => {
                                            e.preventDefault();
                                            setPage(pageNum);
                                        }}
                                    >
                                        {pageNum}
                                    </PaginationLink>
                                </PaginationItem>
                            );
                        })}

                        <PaginationItem>
                            <PaginationNext
                                href="#"
                                onClick={(e) => {
                                    e.preventDefault();
                                    if (locations.page < locations.totalPages) setPage(locations.page + 1);
                                }}
                                className={locations.page === locations.totalPages ? 'pointer-events-none opacity-50' : ''}
                            />
                        </PaginationItem>
                    </PaginationContent>
                </Pagination>
            </div>
        </main>
    )
}