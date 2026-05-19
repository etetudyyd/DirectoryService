"use client";

import DepartmentsList from "@/features/departments/departments-list";
import { DepartmentsViewMode, DepartmentsViewSwitcher } from "@/features/departments/departments-view-switcher";
import DepartmentTree from "@/features/departments/tree/department-tree";
import { useState } from "react";

export default function DepartmentsPage() {
  const [viewMode, setViewMode] =
    useState<DepartmentsViewMode>("tree");

  return (
    <div>
      <div className="flex items-center justify-end px-8">

        <DepartmentsViewSwitcher
          value={viewMode}
          onChange={setViewMode}
        />
      </div>

      {viewMode === "tree" ? (
        <DepartmentTree />
      ) : (
        <DepartmentsList />
      )}
    </div>
  );
}