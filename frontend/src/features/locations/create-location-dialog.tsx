import { Button } from "@/shared/components/ui/button";
import { DialogFooter, DialogHeader, Dialog, DialogContent, DialogDescription, DialogTitle } from "@/shared/components/ui/dialog";
import { Input } from "@/shared/components/ui/input";
import { Label } from "@radix-ui/react-label";
import { useState } from "react";
import { useCreateLocation } from "./model/use-create-location";

type Props = {
    open: boolean;
    onOpenChange: (open: boolean) => void;
}

type CreateFormData = {
    name: string;
    address: {
        country: string;
        region: string;
        city: string;
        street: string;
        house: string;
        postalCode: string;
        apartment: string;
    };
    timeZone: string;
    departmentsIds: string[];
}

export function CreateLocationDialog({open, onOpenChange}: Props) {
const initalData: CreateFormData = {
    name: "",
    address: {
        country: "",
        region: "",
        city: "",
        street: "",
        house: "",
        postalCode: "",
        apartment: ""
    },
    timeZone: "",
    departmentsIds: [] as string[],
};

const [createFormData, setCreateFormData] = useState<CreateFormData>(initalData);

const {createLocation, isPending, error} = useCreateLocation();

const handleSubmit = (event: React.FormEvent) => {
    event.preventDefault();

    createLocation(
        createFormData,
        { onSuccess: () => {
            setCreateFormData(initalData);
            onOpenChange(false);
        }});

}    

return (
    <Dialog open={open} onOpenChange={onOpenChange}>
        <DialogContent>
            <DialogHeader>
                <DialogTitle>Create Location</DialogTitle>
                <DialogDescription>
                    Location creation form goes here.
                </DialogDescription>
            </DialogHeader>

            <form className="grid gap-3 py-4" onSubmit={handleSubmit}>
                <Label className="grid gap-2">
                    <p>Name</p>
                </Label>
                <Input 
                    id="name"
                    type="text"
                    placeholder="Введите название локации"
                    className="w-full"
                    value={createFormData.name}
                    onChange={(e) => setCreateFormData({
                        ...createFormData,
                        name: e.target.value
                    })
                }
                />
        
                <Label className="grid gap-2">
                    <p>Address</p>
                </Label>
                <div className="grid grid-cols-3 gap-2">
                    <Input
                    id="country" 
                    type="text"
                    placeholder="Страна"
                    className="w-full"
                    value={createFormData.address.country}
                    onChange={(e) => setCreateFormData({
                        ...createFormData,
                         address: {...createFormData.address,
                                      country: e.target.value}})}/>
                
                <Input
                    id="region" 
                    type="text"
                    placeholder="Область/регион"
                    className="w-full"
                    value={createFormData.address.region}
                    onChange={(e) => setCreateFormData({
                        ...createFormData,
                        address: {...createFormData.address,
                                    region: e.target.value}})}/>
                
                <Input
                    id="city" 
                    type="text"
                    placeholder="Город"
                    className="w-full"
                    value={createFormData.address.city}
                    onChange={(e) => setCreateFormData({
                        ...createFormData,
                        address: {...createFormData.address,
                                    city: e.target.value}})}/>
                
                <Input
                    id="street" 
                    type="text"
                    placeholder="Улица"
                    className="w-full"
                    value={createFormData.address.street}
                    onChange={(e) => setCreateFormData({
                        ...createFormData,
                        address: {...createFormData.address,
                                    street: e.target.value}})}/>
                
                <Input
                    id="house" 
                    type="text"
                    placeholder="Дом"
                    className="w-full"
                    value={createFormData.address.house}
                    onChange={(e) => setCreateFormData({
                        ...createFormData,
                        address: {...createFormData.address,
                                    house: e.target.value}})}/>
                
                <Input
                    id="postalCode" 
                    type="text"
                    placeholder="Почтовый индекс"
                    className="w-full"
                    value={createFormData.address.postalCode}
                    onChange={(e) => setCreateFormData({
                        ...createFormData,
                        address: {...createFormData.address,
                                    postalCode: e.target.value}})}/>
                
                <Input
                    id="apartment" 
                    type="text"
                    placeholder="Квартира"
                    className="w-full"
                    value={createFormData.address.apartment}
                    onChange={(e) => setCreateFormData({
                        ...createFormData,
                        address: {...createFormData.address,
                                    apartment: e.target.value}})}/>
                
                </div>
                <Label className="grid gap-2">
                    <p>Timezone</p>
                </Label>
                <Input
                    id="timeZone" 
                    type="text"
                    placeholder="Введите таймзону локации"
                    className="w-full"
                    value={createFormData.timeZone}
                    onChange={(e) => setCreateFormData({
                        ...createFormData,
                        timeZone: e.target.value
                    })}/>

                <Label className="grid gap-2">
                    <p>DepartmentsIds</p>
                </Label>
                <Input
                    id="departmentsIds" 
                    type="text"
                    placeholder="Введите ID департаментов через запятую"
                    className="w-full"
                    value={createFormData.departmentsIds.join(",")}
                    onChange={(e) => setCreateFormData({
                        ...createFormData,
                        departmentsIds: e.target.value.split(",").map(id => id.trim())
                    })}/>
            
            <DialogFooter> 
                <Button variant="outline" onClick={() => onOpenChange(false)}>Отмена</Button>
                <Button type="submit" disabled={isPending}>Создать</Button>
                {error && <div className="text-red-500 text-sm">{(error as Error).message}</div>}
            </DialogFooter>
            </form>
        </DialogContent>
    </Dialog>
);
}