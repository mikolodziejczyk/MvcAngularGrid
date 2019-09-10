import { TestBed } from '@angular/core/testing';

import { GridStateStorageServiceService } from './grid-state-storage-service.service';

describe('GridStateStorageServiceService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: GridStateStorageServiceService = TestBed.get(GridStateStorageServiceService);
    expect(service).toBeTruthy();
  });
});
