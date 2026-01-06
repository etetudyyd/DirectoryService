export type Location = {
  id: string;
  name: string;
  address: Address;
  timeZone: string;
  isActive: boolean;
  createdAt: Date;
  updatedAt: Date;
  deletedAt: Date | null;
  
}

export type Address = {
  country: string;
  region: string;
  city: string;
  street: string;
  house: string;
}

