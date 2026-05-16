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

type Props = {
  open: boolean;
  onOpenChange: (open: boolean) => void;
};

const createDepartmentSchema = z.object({
  name: z
    .string()
    .nonempty("Name is cannot be empty")
    .min(3, "Name is has to be at least 3 characters long")
    .max(100, "Name must be at most 100 characters"),
    identifier: z
    .string()
    .nonempty("Identifier cannot be empty")
    .min(3, "Identifier has to be at least 3 characters long")
    .max(100, "Identifier must be at most 100 characters"),
  parentId: z.string().optional().nullable(),
  locationsIds: z.array(z.string().nonempty("Location has to be not empty")),
});

type CreateDepartmentData = z.infer<typeof createDepartmentSchema>;

export function CreateDepartmentDialog({ open, onOpenChange }: Props) {
const initalData: CreateDepartmentData = {
    name: "",
    identifier: "",
    parentId: "",
    locationsIds: [],
  };

   const {
      register,
      handleSubmit,
      reset,
      setValue,
      watch,
      formState: { errors, isValid },
    } = useForm<CreateDepartmentData>({
      defaultValues: initalData,
      resolver: zodResolver(createDepartmentSchema),
    });

     const { createDepartment, isPending, error, isError } = useCreateDepartment();
    
      const locationsIds = watch("locationsIds") || [];
    
    
      const handleLocationsChange = (newLocationsIds: string[]) => {
        setValue("locationsIds", newLocationsIds, {
          shouldValidate: true,
          shouldDirty: true,
        });
      };
    
      const onSubmit = async (data: CreateDepartmentData) => {
        createDepartment({ 
          ...data,
          parentId: data.parentId || null,
          locationsIds: locationsIds,
          },
          {
          onSuccess: () => {
            reset(initalData);
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
                Create New Department
              </DialogTitle>
              <DialogDescription className="text-gray-400">
                Add a new department to the system
              </DialogDescription>
            </div>
          </div>
        </DialogHeader>

        <form
          id="createDepartment"
          className="p-6 space-y-6"
          onSubmit={handleSubmit(onSubmit)}
        >
          {/* Error message */}
          {isError && (
            <div className="p-4 bg-red-900/20 border border-red-800 rounded-lg">
              <div className="flex items-center gap-3">
                <AlertCircleIcon className="h-5 w-5 text-red-400" />
                <div>
                  <p className="text-red-300 font-medium">Creation failed</p>
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
                  className={`bg-gray-900 border-gray-700 text-white placeholder-gray-500 focus:border-blue-500 focus:ring-blue-500/20 ${
                    errors.name
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
                  className={`bg-gray-900 border-gray-700 text-white placeholder-gray-500 focus:border-blue-500 focus:ring-blue-500/20 ${
                    errors.identifier
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

              <div className="space-y-2">
                <Label htmlFor="parentId" className="text-gray-300">
                  Parent Department
                </Label>
                <div className="relative group">
                  <div className="absolute left-3 top-1/2 -translate-y-1/2 z-10">
                    <Building className="h-4 w-4 text-gray-400 group-hover:text-gray-300 transition-colors" />
                  </div>
                  <div className="pl-10">
                <DepartmentItemSelector
                  selectedItemsIds={watch("parentId") ? [watch("parentId") as string] : []}
                  onDepartmentChange={(newParentId) => {
                    setValue("parentId", newParentId[0] || "", {
                      shouldValidate: true,
                      shouldDirty: true,
                    });
                  }}
                  
                />
                </div>
                </div>
              </div>


              <div className="space-y-2">
                <Label htmlFor="locationsIds" className="text-gray-300">
                  Locations
                </Label>
                <div className="relative group">
                  <div className="absolute left-3 top-1/2 -translate-y-1/2 z-10">
                    <LocateIcon className="h-4 w-4 text-gray-400 group-hover:text-gray-300 transition-colors" />
                  </div>
                  <div className="pl-10">
                    <LocationItemSelector
                      selectedItemsIds={locationsIds}
                      onLocationChange={handleLocationsChange}
                    />
                  </div>
                </div>
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
                  Creating...
                </>
              ) : (
                <>
                  <PlusIcon className="h-4 w-4 mr-2" />
                  Create Department
                </>
              )}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>);
    
}