"use client";

import DepartmentPositionsPanel from "@/features/departments/department-positions-panel";
import { useDepartmentsTreeState } from "@/features/departments/model/department-tree-store";
import DepartmentTree from "@/features/departments/tree/department-tree";

export default function DepartmentsPage() {
  const { selectedId } = useDepartmentsTreeState();

  return (
    <main className="mx-auto grid max-w-7xl grid-cols-1 gap-6 px-4 py-6 lg:grid-cols-[380px_minmax(0,1fr)] lg:px-8">
      <section className="rounded-lg border border-slate-800 bg-slate-950/60 p-4">
        <div className="mb-4">
          <h1 className="text-xl font-semibold text-white">Оргструктура</h1>
        </div>

        <DepartmentTree />
      </section>

      <section className="min-w-0 rounded-lg border border-slate-800 bg-slate-950/60 p-4">
        <div className="mb-4">
          <h2 className="text-xl font-semibold text-white">Позиции подразделения</h2>
        </div>

        <DepartmentPositionsPanel departmentId={selectedId} />
      </section>
    </main>
  );
}
