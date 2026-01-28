import { Button } from "@/shared/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/shared/components/ui/dialog";
import { Input } from "@/shared/components/ui/input";
import { Label } from "@/shared/components/ui/label";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import { useCreateLocation } from "./model/use-create-location";
import { BuildingIcon, MapPinIcon, ClockIcon, UsersIcon, AlertCircleIcon, PlusIcon } from "lucide-react";

type Props = {
  open: boolean;
  onOpenChange: (open: boolean) => void;
};

const createLocationSchema = z.object({
  name: z
    .string()
    .nonempty("Name is cannot be empty")
    .min(3, "Name is has to be at least 3 characters long")
    .max(100, "Name must be at most 100 characters"),
  address: z.object({
    region: z.string().nonempty("Region is has to be not empty"),
    city: z.string().nonempty("City is has to be not empty"),
    street: z.string().nonempty("Street is has to be not empty"),
    house: z.string().nonempty("House is has to be not empty"),
    postalCode: z.string().nonempty("Postal code has to be not empty"),
    apartment: z.string().nonempty("Apartment has to be not empty"),
  }),
  timeZone: z.string().nonempty("Timezone has to be not empty"),
  departmentsIds: z.array(z.string().nonempty("Department has to be not empty")),
});

type CreateLocationData = z.infer<typeof createLocationSchema>;

export function CreateLocationDialog({ open, onOpenChange }: Props) {
  const initalData: CreateLocationData = {
    name: "",
    address: {
      region: "",
      city: "",
      street: "",
      house: "",
      postalCode: "",
      apartment: "",
    },
    timeZone: "",
    departmentsIds: [] as string[],
  };

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors, isValid },
  } = useForm<CreateLocationData>({
    defaultValues: initalData,
    resolver: zodResolver(createLocationSchema),
  });

  const { createLocation, isPending, error, isError } = useCreateLocation();

  const onSubmit = async (data: CreateLocationData) => {
    createLocation(data, {
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
                Create New Location
              </DialogTitle>
              <DialogDescription className="text-gray-400">
                Add a new location to the system
              </DialogDescription>
            </div>
          </div>
        </DialogHeader>

        <form
          id="createLocation"
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
            <div className="flex items-center gap-2">
              <BuildingIcon className="h-5 w-5 text-blue-400" />
              <h3 className="text-lg font-semibold text-white">Basic Information</h3>
            </div>
            
            <div className="grid gap-4">
              <div className="space-y-2">
                <Label htmlFor="name" className="text-gray-300">
                  Location Name *
                </Label>
                <Input
                  id="name"
                  type="text"
                  placeholder="Enter location name (e.g., Main Office, Warehouse)"
                  className={`bg-gray-900 border-gray-700 text-white placeholder-gray-500 focus:border-blue-500 focus:ring-blue-500/20 ${
                    errors.name ? "border-red-500 focus:border-red-500 focus:ring-red-500/20" : ""
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
                <Label htmlFor="timeZone" className="text-gray-300 flex items-center gap-2">
                  <ClockIcon className="h-4 w-4" />
                  Timezone *
                </Label>
                <Input
                  id="timeZone"
                  type="text"
                  placeholder="e.g., Europe/London, America/New_York"
                  className={`bg-gray-900 border-gray-700 text-white placeholder-gray-500 focus:border-blue-500 focus:ring-blue-500/20 ${
                    errors.timeZone ? "border-red-500 focus:border-red-500 focus:ring-red-500/20" : ""
                  }`}
                  {...register("timeZone")}
                />
                {errors.timeZone && (
                  <p className="text-red-400 text-sm flex items-center gap-1">
                    <AlertCircleIcon className="h-4 w-4" />
                    {errors.timeZone.message}
                  </p>
                )}
              </div>
            </div>
          </div>

          {/* Address Information */}
          <div className="space-y-4">
            <div className="flex items-center gap-2">
              <MapPinIcon className="h-5 w-5 text-green-400" />
              <h3 className="text-lg font-semibold text-white">Address Details</h3>
            </div>
            
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div className="space-y-2">
                <Label htmlFor="region" className="text-gray-300 text-sm">
                  Region/State *
                </Label>
                <Input
                  id="region"
                  type="text"
                  placeholder="Region or State"
                  className={`bg-gray-900 border-gray-700 text-white placeholder-gray-500 focus:border-blue-500 focus:ring-blue-500/20 ${
                    errors.address?.region ? "border-red-500 focus:border-red-500 focus:ring-red-500/20" : ""
                  }`}
                  {...register("address.region")}
                />
              </div>

              <div className="space-y-2">
                <Label htmlFor="city" className="text-gray-300 text-sm">
                  City *
                </Label>
                <Input
                  id="city"
                  type="text"
                  placeholder="City"
                  className={`bg-gray-900 border-gray-700 text-white placeholder-gray-500 focus:border-blue-500 focus:ring-blue-500/20 ${
                    errors.address?.city ? "border-red-500 focus:border-red-500 focus:ring-red-500/20" : ""
                  }`}
                  {...register("address.city")}
                />
              </div>

              <div className="space-y-2 md:col-span-2">
                <Label htmlFor="street" className="text-gray-300 text-sm">
                  Street Address *
                </Label>
                <Input
                  id="street"
                  type="text"
                  placeholder="Street address"
                  className={`bg-gray-900 border-gray-700 text-white placeholder-gray-500 focus:border-blue-500 focus:ring-blue-500/20 ${
                    errors.address?.street ? "border-red-500 focus:border-red-500 focus:ring-red-500/20" : ""
                  }`}
                  {...register("address.street")}
                />
              </div>

              <div className="space-y-2">
                <Label htmlFor="house" className="text-gray-300 text-sm">
                  House Number *
                </Label>
                <Input
                  id="house"
                  type="text"
                  placeholder="House number"
                  className={`bg-gray-900 border-gray-700 text-white placeholder-gray-500 focus:border-blue-500 focus:ring-blue-500/20 ${
                    errors.address?.house ? "border-red-500 focus:border-red-500 focus:ring-red-500/20" : ""
                  }`}
                  {...register("address.house")}
                />
              </div>

              <div className="space-y-2">
                <Label htmlFor="postalCode" className="text-gray-300 text-sm">
                  Postal Code *
                </Label>
                <Input
                  id="postalCode"
                  type="text"
                  placeholder="ZIP/Postal code"
                  className={`bg-gray-900 border-gray-700 text-white placeholder-gray-500 focus:border-blue-500 focus:ring-blue-500/20 ${
                    errors.address?.postalCode ? "border-red-500 focus:border-red-500 focus:ring-red-500/20" : ""
                  }`}
                  {...register("address.postalCode")}
                />
              </div>

              <div className="space-y-2">
                <Label htmlFor="apartment" className="text-gray-300 text-sm">
                  Apartment/Unit *
                </Label>
                <Input
                  id="apartment"
                  type="text"
                  placeholder="Apartment, suite, or unit"
                  className={`bg-gray-900 border-gray-700 text-white placeholder-gray-500 focus:border-blue-500 focus:ring-blue-500/20 ${
                    errors.address?.apartment ? "border-red-500 focus:border-red-500 focus:ring-red-500/20" : ""
                  }`}
                  {...register("address.apartment")}
                />
              </div>
            </div>

            {/* Address errors */}
            {errors.address && (
              <div className="p-3 bg-red-900/20 border border-red-800 rounded-lg">
                <div className="grid grid-cols-1 md:grid-cols-2 gap-2 text-sm">
                  {errors.address.region && (
                    <p className="text-red-400 flex items-center gap-1">
                      <AlertCircleIcon className="h-3 w-3" />
                      Region: {errors.address.region.message}
                    </p>
                  )}
                  {errors.address.city && (
                    <p className="text-red-400 flex items-center gap-1">
                      <AlertCircleIcon className="h-3 w-3" />
                      City: {errors.address.city.message}
                    </p>
                  )}
                  {errors.address.street && (
                    <p className="text-red-400 flex items-center gap-1">
                      <AlertCircleIcon className="h-3 w-3" />
                      Street: {errors.address.street.message}
                    </p>
                  )}
                  {errors.address.house && (
                    <p className="text-red-400 flex items-center gap-1">
                      <AlertCircleIcon className="h-3 w-3" />
                      House: {errors.address.house.message}
                    </p>
                  )}
                  {errors.address.postalCode && (
                    <p className="text-red-400 flex items-center gap-1">
                      <AlertCircleIcon className="h-3 w-3" />
                      Postal Code: {errors.address.postalCode.message}
                    </p>
                  )}
                  {errors.address.apartment && (
                    <p className="text-red-400 flex items-center gap-1">
                      <AlertCircleIcon className="h-3 w-3" />
                      Apartment: {errors.address.apartment.message}
                    </p>
                  )}
                </div>
              </div>
            )}
          </div>

          {/* Departments */}
          <div className="space-y-4">
            <div className="flex items-center gap-2">
              <UsersIcon className="h-5 w-5 text-purple-400" />
              <h3 className="text-lg font-semibold text-white">Departments</h3>
            </div>
            
            <div className="space-y-2">
              <Label htmlFor="departmentsIds" className="text-gray-300">
                Department IDs (comma-separated)
              </Label>
              <Input
                id="departmentsIds"
                type="text"
                placeholder="e.g., dept-001, dept-002, dept-003 (optional)"
                className={`bg-gray-900 border-gray-700 text-white placeholder-gray-500 focus:border-blue-500 focus:ring-blue-500/20 ${
                  errors.departmentsIds ? "border-red-500 focus:border-red-500 focus:ring-red-500/20" : ""
                }`}
                {...register("departmentsIds", {
                  setValueAs: (value) => {
                    if (typeof value !== "string") return [];
                    return value
                      .split(",")
                      .map((id) => id.trim())
                      .filter(Boolean);
                  },
                })}
              />
              <p className="text-gray-500 text-sm">
                Enter department IDs separated by commas. Leave empty if no departments assigned yet.
              </p>
              {errors.departmentsIds && (
                <p className="text-red-400 text-sm flex items-center gap-1">
                  <AlertCircleIcon className="h-4 w-4" />
                  {errors.departmentsIds.message}
                </p>
              )}
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
                  Create Location
                </>
              )}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}