"use client";

import { Tabs, TabsList, TabsTrigger } from "@/shared/components/ui/tabs";
import { List, Network } from "lucide-react";

export type DepartmentsViewMode = "list" | "tree";

type DepartmentsViewSwitcherProps = {
  value: DepartmentsViewMode;
  onChange: (value: DepartmentsViewMode) => void;
};

export function DepartmentsViewSwitcher({
  value,
  onChange,
}: DepartmentsViewSwitcherProps) {
  return (
    <Tabs
      value={value}
      onValueChange={(value) =>
        onChange(value as DepartmentsViewMode)
      }
    >
      <TabsList>
        <TabsTrigger
          value="tree"
          className="flex items-center gap-2"
        >
          <Network className="h-4 w-4" />
          Tree
        </TabsTrigger>

        <TabsTrigger
          value="list"
          className="flex items-center gap-2"
        >
          <List className="h-4 w-4" />
          List
        </TabsTrigger>
      </TabsList>
    </Tabs>
  );
}