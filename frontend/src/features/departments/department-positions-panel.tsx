"use client";

import { BriefcaseBusiness, MousePointer2 } from "lucide-react";

import { Badge } from "@/shared/components/ui/badge";
import { Card, CardContent, CardHeader } from "@/shared/components/ui/card";
import { Spinner } from "@/shared/components/ui/spinner";
import { usePositionsList } from "@/features/positions/model/use-positions-list";

type DepartmentPositionsPanelProps = {
  departmentId: string | null;
};

export default function DepartmentPositionsPanel({
  departmentId,
}: DepartmentPositionsPanelProps) {
  const {
    positions = [],
    totalItems,
    isPending,
    isError,
    error,
    cursorRef,
    isFetchingNextPage,
  } = usePositionsList({
    departmentsIds: departmentId ? [departmentId] : [],
    search: "",
    isActive: undefined,
    pageSize: 20,
    enabled: Boolean(departmentId),
  });

  if (!departmentId) {
    return (
      <div className="flex min-h-72 flex-col items-center justify-center rounded-lg border border-dashed border-slate-700 bg-slate-950/40 px-6 text-center">
        <MousePointer2 className="mb-4 h-10 w-10 text-slate-500" />
        <h2 className="text-lg font-semibold text-white">Выберите подразделение</h2>
        <p className="mt-2 max-w-md text-sm text-slate-400">
          После выбора узла справа появятся позиции именно этого подразделения.
        </p>
      </div>
    );
  }

  if (isPending) {
    return (
      <div className="flex min-h-72 flex-col items-center justify-center">
        <Spinner className="mb-3 h-10 w-10 text-blue-700" />
        <p className="text-sm text-slate-400">Загружаем позиции...</p>
      </div>
    );
  }

  if (isError) {
    return (
      <div className="rounded-lg border border-red-900 bg-red-950/30 p-4 text-red-200">
        <p className="font-medium">Не удалось загрузить позиции</p>
        <p className="mt-1 text-sm text-red-300">{error?.message}</p>
      </div>
    );
  }

  if (positions.length === 0) {
    return (
      <div className="flex min-h-72 flex-col items-center justify-center rounded-lg border border-dashed border-slate-700 bg-slate-950/40 px-6 text-center">
        <BriefcaseBusiness className="mb-4 h-10 w-10 text-slate-500" />
        <h2 className="text-lg font-semibold text-white">Позиций нет</h2>
        <p className="mt-2 text-sm text-slate-400">
          Для выбранного подразделения позиции не найдены.
        </p>
      </div>
    );
  }

  return (
    <div>
      <div className="mb-4 flex items-center justify-between gap-4">
        <p className="text-sm text-slate-400">
          Показано{" "}
          <span className="font-semibold text-white">{positions.length}</span>
          {typeof totalItems === "number" && (
            <>
              {" "}
              из <span className="font-semibold text-white">{totalItems}</span>
            </>
          )}
        </p>
      </div>

      <div className="grid grid-cols-1 gap-4 xl:grid-cols-2">
        {positions.map((position) => (
          <Card
            key={position.id}
            className="border-slate-800 bg-slate-950/60"
          >
            <CardHeader className="space-y-3 pb-3">
              <div className="flex items-start justify-between gap-3">
                <h3 className="min-w-0 truncate text-base font-semibold text-white">
                  {position.name}
                </h3>
                <Badge
                  variant="outline"
                  className={
                    position.isActive
                      ? "border-emerald-800 text-emerald-300"
                      : "border-red-900 text-red-300"
                  }
                >
                  {position.isActive ? "Active" : "Inactive"}
                </Badge>
              </div>
            </CardHeader>
            <CardContent>
              <p className="line-clamp-3 text-sm text-slate-400">
                {position.description || "Описание не заполнено"}
              </p>
            </CardContent>
          </Card>
        ))}
      </div>

      <div ref={cursorRef} className="py-6">
        {isFetchingNextPage && (
          <div className="flex justify-center">
            <div className="inline-flex items-center gap-3 rounded-full bg-slate-900 px-5 py-3">
              <Spinner className="h-5 w-5 text-blue-700" />
              <span className="text-sm text-slate-400">Загружаем еще...</span>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}
