import { DictionaryItemResponse } from "@/shared/api/types";
import { Spinner } from "@/shared/components/ui/spinner";
import { MultiSelect } from "@/shared/components/ui/multi-select";
import React from "react";
import { useDepartmentDictionary } from "../../features/departments/model/use-departments-dictionary";

const PAGE_SIZE = 3;
type Props = {
  selectedItemsIds?: string[];
  excludeItemsIds?: string[];
  onDepartmentChange: (itemIds: string[]) => void;
};

export default function DepartmentItemSelector({
  selectedItemsIds: selectedItemsIds,
  excludeItemsIds: excludeItemsIds,
  onDepartmentChange: onItemChange,
}: Props) {
  const {
    items: selectedItems,
    isPending: isSelectedPending,
    isError: isSelectedError,
  } = useDepartmentDictionary({
    pageSize: PAGE_SIZE,
    departmentsIds:
      selectedItemsIds && selectedItemsIds.length > 0
        ? selectedItemsIds
        : undefined,
  });
  const {
    items: allItems,
    isPending: isAllPending,
    isError: isAllError,
    error: allError,
    isFetchingNextPage: isAllFetchingNextPage,
    hasNextPage: isAllHasNextPage,
    fetchNextPage: allFetchNextPage,
  } = useDepartmentDictionary({
    pageSize: PAGE_SIZE,
  });

  const combinedItems = React.useMemo(() => {
    const allItemsMap = new Map<string, boolean>();
    const result: DictionaryItemResponse[] = [];

    if (selectedItems) {
      selectedItems.forEach((item) => {
        if (!allItemsMap.has(item.id)) {
          result.push(item);
          allItemsMap.set(item.id, true);
        }
      });
    }

    if (allItems) {
      allItems.forEach((dept) => {
        if (!allItemsMap.has(dept.id)) {
          result.push(dept);
          allItemsMap.set(dept.id, true);
        }
      });
    }

    let filteredResult = result;
    if (excludeItemsIds && excludeItemsIds.length > 0) {
      filteredResult = result.filter((dept) => !excludeItemsIds.includes(dept.id));
    }

    return filteredResult;
  }, [selectedItems, allItems, excludeItemsIds]);

  const multiSelectOptions = React.useMemo(() => {
    return combinedItems.map((dept) => ({
      value: dept.id,
      label: dept.name,
    }));
  }, [combinedItems]);

  const filteredDefaultValues = React.useMemo(() => {
    if (!selectedItemsIds || !selectedItemsIds.length) return [];
    const availableIds = new Set(multiSelectOptions.map(option => option.value));
    return selectedItemsIds.filter(id => availableIds.has(id));
  }, [selectedItemsIds, multiSelectOptions]);

  const showComponent =
    !isAllPending &&
    !isAllError &&
    !isSelectedPending &&
    !isSelectedError &&
    combinedItems &&
    combinedItems.length > 0;

  return (
    <>
      {isAllPending && <Spinner />}
      {!isAllPending && allError && (
        <p className="text-red-500">
          Error of loading items: {allError?.message}
        </p>
      )}
      {showComponent && (
        <MultiSelect
          className="flex-1"
          options={multiSelectOptions}
          onValueChange={onItemChange}
          defaultValue={filteredDefaultValues}
          placeholder="Select departments"
          searchPlaceholder="Search..."
          morePlaceholder="more"
          notFoundPlaceholder="empty"
          disabled={isAllFetchingNextPage}
          loadMore={allFetchNextPage}
          hasNextPage={isAllHasNextPage}
          isLoadingMore={isAllFetchingNextPage}
          maxDisplayCount={1}
        />
      )}
      {!isAllPending &&
        !isAllError &&
        !isSelectedPending &&
        !isSelectedError &&
        combinedItems &&
        combinedItems.length === 0 && (
          <p className="text-muted-foreground">
            No departments available
          </p>
        )}
    </>
  );
}