import { Skeleton } from "@/shared/components/ui/skeleton";

export function TreeSkeleton() {
  return (
    <div className="space-y-2 py-2 pl-8">
      {Array.from({ length: 3 }).map(
        (_, index) => (
          <Skeleton
            key={index}
            className="h-8 w-[240px]"
          />
        ),
      )}
    </div>
  );
}