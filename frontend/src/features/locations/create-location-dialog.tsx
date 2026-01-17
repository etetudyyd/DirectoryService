import { Button } from "@/shared/components/ui/button";
import { DialogFooter, DialogHeader, Dialog, DialogContent, DialogDescription, DialogTitle } from "@/shared/components/ui/dialog";
import { Input } from "@/shared/components/ui/input";
import { Label } from "@radix-ui/react-label";

type Props = {
    open: boolean;
    onOpenChange: (open: boolean) => void;
}

export function CreateLocationDialog({open, onOpenChange}: Props) {
return (
    <Dialog open={open} onOpenChange={onOpenChange}>
        <DialogContent>
            <DialogHeader>
                <DialogTitle>Create Location</DialogTitle>
                <DialogDescription>
                    Location creation form goes here.
                </DialogDescription>
            </DialogHeader>

            <div className="grid gap-4 py-4">
                <Label className="grid gap-2">
                    название локации
                </Label>
                <Input 
                    type="text"
                    placeholder="Введите название локации"
                    className="w-full"/>
            </div>
            <div className="grid gap-4 py-4">
                <Label className="grid gap-2">
                    адресс локации
                </Label>
                <Input 
                    type="text"
                    placeholder="Введите адресс локации"
                    className="w-full"/>
                
            </div>
            <div className="grid gap-4 py-4">
                <Label className="grid gap-2">
                    таймзону локации
                </Label>
                <Input 
                    type="text"
                    placeholder="Введите таймзону локации"
                    className="w-full"/>
                
            </div>
            <DialogFooter> 
                <Button variant="outline" onClick={() => onOpenChange(false)}>Отмена</Button>
                <Button type="submit">Создать</Button>
            </DialogFooter>
        </DialogContent>
    </Dialog>
);
}