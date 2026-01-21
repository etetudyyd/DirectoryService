export type Location = {
  id: string;
  name: string;
  address: Address;
  timeZone: string;
  isActive: boolean;
  createdAt: Date;
  updatedAt: Date;
  deletedAt: Date | null;
  departmentsIds: string[];
}

export type Address = {
  country: string;
  region: string;
  city: string;
  street: string;
  house: string;
  postalCode: string;
  apartment: string;
}

