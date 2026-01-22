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
import { Location } from "@/entities/locations/types";
import { useUpdateLocation } from "./model/use-update-location";

const updateLocationSchema = z.object({
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
  departmentsIds: z.array(
    z.string().nonempty("Department has to be not empty"),
  ),
});

type Props = {
  location: Location;
  open: boolean;
  onOpenChange: (open: boolean) => void;
};

type UpdateLocationData = z.infer<typeof updateLocationSchema>;

export function UpdateLocationDialog({ location, open, onOpenChange }: Props) {
  const {
    register,
    handleSubmit,
    formState: { errors, isValid },
  } = useForm<UpdateLocationData>({
    defaultValues: {
      name: location.name,
      address: location.address,
      timeZone: location.timeZone,
      departmentsIds: location.departmentsIds,
    },
    resolver: zodResolver(updateLocationSchema),
  });

  const { updateLocation, isPending, error, isError } = useUpdateLocation();

  const onSubmit = (data: UpdateLocationData) => {
    updateLocation(
      { locationId: location.id, ...data },
      {
        onSuccess: () => {
          onOpenChange(false);
        },
      },
    );
  };

  const getErrorMessage = (): string => {
    if (isError) {
      return error ? error.message : "Undefined error";
    }

    return "";
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className=" max-h-[80vh] overflow-y-auto p-6">
        <DialogHeader>
          <DialogTitle>Update Location</DialogTitle>
          <DialogDescription>Location update form goes here.</DialogDescription>
        </DialogHeader>

        <form
          id="updateLocation"
          className="grid gap-3 py-4"
          onSubmit={handleSubmit(onSubmit)}
        >
          <div className="grid gap-2">
            <Label htmlFor="name">
              <p>Name</p>
            </Label>
            <Input
              id="name"
              type="text"
              placeholder="Введите название локации"
              className={`w-full ${
                errors.name
                  ? "border-destructive focus-visible:ring-destructive"
                  : ""
              }`}
              {...register("name")}
            />
            {errors.name && (
              <div className="text-destructive text-sm">
                {errors.name.message}
              </div>
            )}
          </div>

          <div className="grid gap-2">
            <Label htmlFor="address">
              <p>Address</p>
            </Label>
            <div className="grid grid-cols-3 gap-2">
              <Input
                id="region"
                type="text"
                placeholder="Область/регион"
                className={`w-full ${
                  errors.address?.region
                    ? "border-destructive focus-visible:ring-destructive"
                    : ""
                }`}
                {...register("address.region")}
              />

              <Input
                id="city"
                type="text"
                placeholder="Город"
                className={`w-full ${
                  errors.address?.city
                    ? "border-destructive focus-visible:ring-destructive"
                    : ""
                }`}
                {...register("address.city")}
              />

              <Input
                id="street"
                type="text"
                placeholder="Улица"
                className={`w-full ${
                  errors.address?.street
                    ? "border-destructive focus-visible:ring-destructive"
                    : ""
                }`}
                {...register("address.street")}
              />

              <Input
                id="house"
                type="text"
                placeholder="Дом"
                className={`w-full ${
                  errors.address?.house
                    ? "border-destructive focus-visible:ring-destructive"
                    : ""
                }`}
                {...register("address.house")}
              />

              <Input
                id="postalCode"
                type="text"
                placeholder="Почтовый индекс"
                className={`w-full ${
                  errors.address?.postalCode
                    ? "border-destructive focus-visible:ring-destructive"
                    : ""
                }`}
                {...register("address.postalCode")}
              />

              <Input
                id="apartment"
                type="text"
                placeholder="Квартира"
                className={`w-full ${
                  errors.address?.apartment
                    ? "border-destructive focus-visible:ring-destructive"
                    : ""
                }`}
                {...register("address.apartment")}
              />

              {errors.address && (
                <div className="text-destructive text-sm col-span-3">
                  {errors.address.region && (
                    <div>{errors.address.region.message}</div>
                  )}
                  {errors.address.city && (
                    <div>{errors.address.city.message}</div>
                  )}
                  {errors.address.street && (
                    <div>{errors.address.street.message}</div>
                  )}
                  {errors.address.house && (
                    <div>{errors.address.house.message}</div>
                  )}
                  {errors.address.postalCode && (
                    <div>{errors.address.postalCode.message}</div>
                  )}
                  {errors.address.apartment && (
                    <div>{errors.address.apartment.message}</div>
                  )}
                </div>
              )}
            </div>
          </div>

          <div className="grid gap-2">
            <Label htmlFor="timeZone">
              <p>Timezone</p>
            </Label>
            <Input
              id="timeZone"
              type="text"
              placeholder="Введите таймзону локации"
              className={`w-full ${
                errors.timeZone
                  ? "border-destructive focus-visible:ring-destructive"
                  : ""
              }`}
              {...register("timeZone")}
            />
            {errors.timeZone && (
              <div className="text-destructive text-sm">
                {errors.timeZone.message}
              </div>
            )}
          </div>

          <div className="grid gap-2">
            <Label htmlFor="departmentsIds">
              <p>Departments</p>
            </Label>
            <Input
              id="departmentsIds"
              type="text"
              placeholder="Введите ID департаментов через запятую"
              className={`w-full ${
                errors.departmentsIds
                  ? "border-destructive focus-visible:ring-destructive"
                  : ""
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
            {errors.departmentsIds && (
              <div className="text-destructive text-sm">
                {errors.departmentsIds.message}
              </div>
            )}
          </div>

          <DialogFooter className="pt-2">
            {error && (
              <div className="text-sm text-red-500 mt-2 mr-auto">
                {getErrorMessage()}
              </div>
            )}

            <Button disabled={isPending || !isValid} type="submit">
              {isPending ? "Saving..." : "Save"}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
