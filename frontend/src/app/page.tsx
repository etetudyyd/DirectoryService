import { Button } from "@/shared/components/ui/button";
import Image from "next/image";
import Link from "next/link";

export default function Home() {
  return (
      <main>
        {/* Hero */}
        <section className="rounded-lg bg-linear-to-r from-white/5 to-white/2 p-8 mb-6">
          <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-6">
            <div>
              <h1 className="text-3xl md:text-4xl font-extrabold">Welcome to Directory Service</h1>
              <p className="mt-2 text-muted-foreground max-w-xl">
                Manage your organization's positions, departments, and locations quickly and easily.
              </p>

                <div className="mt-4 flex items-center gap-3">
                <Button variant="default">Get Started</Button>
                <Link href="/contacts" className="text-sm text-primary-400 hover:underline">View positions</Link>
              </div>
            </div>       
          </div>
        </section>

        {/* Stats */}
        <section className="grid grid-cols-1 sm:grid-cols-3 gap-4 mb-6">
          <div className="p-4 rounded-lg bg-muted">
            <div className="text-sm text-muted-foreground">Positions</div>
            <div className="text-2xl font-bold">1,248</div>
          </div>
          <div className="p-4 rounded-lg bg-muted">
            <div className="text-sm text-muted-foreground">Departments</div>
            <div className="text-2xl font-bold">24</div>
          </div>
          <div className="p-4 rounded-lg bg-muted">
            <div className="text-sm text-muted-foreground">Locations</div>
            <div className="text-2xl font-bold">8</div>
          </div>
        </section>

        {/* Quick actions + Recent */}
        <section className="grid grid-cols-1 lg:grid-cols-3 gap-6">
          <div className="lg:col-span-2 space-y-4">
            <div className="flex items-center justify-between">
              <h2 className="text-lg font-semibold">Quick actions</h2>
            </div>

            <div className="flex flex-wrap gap-3">
              <Button variant="ghost">Add Position</Button>
              <Button variant="outline">Import CSV</Button>
              <Button variant="outline">Export</Button>
            </div>

            <div className="mt-4">
              <h3 className="text-sm font-medium mb-2">Recent positions</h3>
              <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
                <div className="p-3 rounded-md bg-background shadow">
                  <div className="flex items-center justify-between">
                    <div>
                      <div className="font-medium">Ivan Petrov</div>
                      <div className="text-xs text-muted-foreground">Engineering — ivan.petrov@example.com</div>
                    </div>
                    <div className="text-sm text-muted-foreground">Dept: Dev</div>
                  </div>
                </div>
                <div className="p-3 rounded-md bg-background shadow">
                  <div className="flex items-center justify-between">
                    <div>
                      <div className="font-medium">Olga Smirnova</div>
                      <div className="text-xs text-muted-foreground">HR — olga.smirnova@example.com</div>
                    </div>
                    <div className="text-sm text-muted-foreground">Dept: HR</div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <aside className="space-y-4">
            <div className="p-4 rounded-lg bg-muted">
              <h4 className="text-sm font-medium">Updates</h4>
              <ul className="mt-2 text-sm text-muted-foreground list-disc list-inside">
                <li>Sync completed</li>
                <li>New privacy policy</li>
              </ul>
            </div>

            <div className="p-4 rounded-lg bg-muted">
              <h4 className="text-sm font-medium">Tip</h4>
              <p className="text-sm text-muted-foreground mt-2">Use search to quickly access positions by name or department.</p>
            </div>
          </aside>
        </section>
      </main>
  );
}
