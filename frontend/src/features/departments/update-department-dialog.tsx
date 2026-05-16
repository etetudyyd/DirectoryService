import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import { useCreateDepartment } from "./model/use-create-department";
import { Dialog, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle } from "@/shared/components/ui/dialog";
import { Button } from "@/shared/components/ui/button";
import { Input } from "@/shared/components/ui/input";
import { PlusIcon, AlertCircleIcon, Building, LocateIcon } from "lucide-react";
import LocationItemSelector from "@/widgets/locations/locations-item-selector";
import { Label } from "@/shared/components/ui/label";
import DepartmentItemSelector from "@/widgets/departments/department-item-selector";
import { Department, DepartmentDetails } from "@/entities/departments/types";
import { useUpdateDepartment } from "./model/use-update-department";

type Props = {
    department: Department | DepartmentDetails;
    open: boolean;
    onOpenChange: (open: boolean) => void;
};

const updateDepartmentSchema = z.object({
    name: z
        .string()
        .nonempty("Name is cannot be empty")
        .min(3, "Name is has to be at least 3 characters long")
        .max(100, "Name must be at most 100 characters"),
    identifier: z
        .string()
        .nonempty("Identifier cannot be empty")
        .min(3, "Identifier has to be at least 3 characters long")
        .max(100, "Identifier must be at most 100 characters")
});

type UpdateDepartmentData = z.infer<typeof updateDepartmentSchema>;

export function UpdateDepartmentDialog({ department, open, onOpenChange }: Props) {

    const {
        register,
        handleSubmit,
        formState: { errors, isValid },
    } = useForm<UpdateDepartmentData>({
        defaultValues: {
            name: department.name,
            identifier: department.identifier
        },
        resolver: zodResolver(updateDepartmentSchema),
    });

    const { updateDepartment, isPending, error, isError } = useUpdateDepartment();


    const onSubmit = async (data: UpdateDepartmentData) => {
        updateDepartment({
            departmentId: department.id,
            ...data,
        },
            {
                onSuccess: () => {
                    onOpenChange(false);
                },
            });
    };

    return (
        <Dialog open={open} onOpenChange={onOpenChange}>
            <DialogContent className="max-w-2xl max-h-[90vh] overflow-y-auto p-0 bg-linear-to-b from-gray-900 to-black border-gray-800">
                {/* Header */}
                <DialogHeader className="p-6 pb-4 border-b border-gray-800 bg-gray-900/50">
                    <div className="flex items-center gap-3">
                        <div className="h-10 w-10 rounded-lg bg-linear-to-br from-green-600 to-emerald-800 flex items-center justify-center">
                            <PlusIcon className="h-5 w-5 text-white" />
                        </div>
                        <div>
                            <DialogTitle className="text-xl font-bold text-white">
                                Update Department
                            </DialogTitle>
                            <DialogDescription className="text-gray-400">
                                Update department information
                            </DialogDescription>
                        </div>
                    </div>
                </DialogHeader>

                <form
                    id="updateDepartment"
                    className="p-6 space-y-6"
                    onSubmit={handleSubmit(onSubmit)}
                >
                    {/* Error message */}
                    {isError && (
                        <div className="p-4 bg-red-900/20 border border-red-800 rounded-lg">
                            <div className="flex items-center gap-3">
                                <AlertCircleIcon className="h-5 w-5 text-red-400" />
                                <div>
                                    <p className="text-red-300 font-medium">Update failed</p>
                                    <p className="text-red-400 text-sm mt-1">
                                        {error?.message || "An error occurred. Please try again."}
                                    </p>
                                </div>
                            </div>
                        </div>
                    )}

                    {/* Basic Information */}
                    <div className="space-y-4">
                        <div className="grid gap-4">
                            <div className="space-y-2">
                                <Label htmlFor="name" className="text-gray-300">
                                    Name *
                                </Label>
                                <Input
                                    id="name"
                                    type="text"
                                    placeholder="Enter department name"
                                    className={`bg-gray-900 border-gray-700 text-white placeholder-gray-500 focus:border-blue-500 focus:ring-blue-500/20 ${errors.name
                                            ? "border-red-500 focus:border-red-500 focus:ring-red-500/20"
                                            : ""
                                        }`}
                                    {...register("name")}
                                />
                                {errors.name && (
                                    <p className="text-red-400 text-sm flex items-center gap-1">
                                        <AlertCircleIcon className="h-4 w-4" />
                                        {errors.name.message}
                                    </p>
                                )}
                            </div>

                            <div className="space-y-2">
                                <Label htmlFor="identifier" className="text-gray-300">
                                    Identifier *
                                </Label>
                                <Input
                                    id="identifier"
                                    type="text"
                                    placeholder="Enter identifier"
                                    className={`bg-gray-900 border-gray-700 text-white placeholder-gray-500 focus:border-blue-500 focus:ring-blue-500/20 ${errors.identifier
                                            ? "border-red-500 focus:border-red-500 focus:ring-red-500/20"
                                            : ""
                                        }`}
                                    {...register("identifier")}
                                />
                                {errors.identifier && (
                                    <p className="text-red-400 text-sm flex items-center gap-1">
                                        <AlertCircleIcon className="h-4 w-4" />
                                        {errors.identifier.message}
                                    </p>
                                )}
                            </div>
                        </div>
                    </div>

                    {/* Footer */}
                    <DialogFooter className="pt-6 border-t border-gray-800">
                        <Button
                            type="button"
                            variant="outline"
                            onClick={() => onOpenChange(false)}
                            className="border-gray-700 text-gray-300 hover:bg-gray-800 hover:text-white"
                        >
                            Cancel
                        </Button>
                        <Button
                            type="submit"
                            disabled={isPending || !isValid}
                            className="bg-linear-to-r from-green-600 to-emerald-700 hover:from-green-700 hover:to-emerald-800 text-white shadow-lg hover:shadow-xl transition-all duration-200 disabled:opacity-50 disabled:cursor-not-allowed"
                        >
                            {isPending ? (
                                <>
                                    <div className="h-4 w-4 border-2 border-white/30 border-t-white rounded-full animate-spin mr-2" />
                                    Updating...
                                </>
                            ) : (
                                <>
                                    <PlusIcon className="h-4 w-4 mr-2" />
                                    Update Department
                                </>
                            )}
                        </Button>
                    </DialogFooter>
                </form>
            </DialogContent>
        </Dialog>);

}