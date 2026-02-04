import { useState } from "react";
import { Badge } from "@/shared/components/ui/badge";
import { Button } from "@/shared/components/ui/button";
import {
  Command,
  CommandEmpty,
  CommandGroup,
  CommandInput,
  CommandItem,
} from "@/shared/components/ui/command";
import { Label } from "@/shared/components/ui/label";
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@/shared/components/ui/popover";
import { ScrollArea } from "@/shared/components/ui/scroll-area";
import { AlertCircleIcon, Check, ChevronsUpDown, X } from "lucide-react";

type DepartmentProps = {
  departmentIds: string[];
  onDepartmentChange: (departmentIds: string[]) => void;
  errors?: {
    departmentsIds?: {
      message?: string;
    };
  };
  placeholder?: string;
  className?: string;
};

export function DepartmentsMultiSelectItem(){
  
}
  