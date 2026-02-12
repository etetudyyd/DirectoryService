import {
  Alert,
  AlertDescription,
  AlertTitle,
} from "@/shared/components/ui/alert";
import { AlertCircle } from "lucide-react";
import { Button } from "@/shared/components/ui/button";

export function DetailsErrorPage({
  error,
  onRetry,
}: {
  error: Error;
  onRetry: () => void;
}) {
  return (
    <div className="container mx-auto py-6">
      <Alert variant="destructive">
        <AlertCircle className="h-4 w-4" />
        <AlertTitle>Error loading position</AlertTitle>
        <AlertDescription className="space-y-4">
          <p>{error.message}</p>
          <div className="flex gap-2">
            <Button variant="outline" size="sm" onClick={onRetry}>
              Try Again
            </Button>
            <Button
              variant="ghost"
              size="sm"
              onClick={() => window.history.back()}
            >
              Go Back
            </Button>
          </div>
        </AlertDescription>
      </Alert>
    </div>
  );
}
