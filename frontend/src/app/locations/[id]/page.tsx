"use client";

export default function LocationPage({ params }: { params: { id: string } }) {
    return (
    <div className="container mx-auto py-8">
        <h1 className="text-2xl font-bold mb-4">Location {params.id}</h1>
    </div>
    
);
}
