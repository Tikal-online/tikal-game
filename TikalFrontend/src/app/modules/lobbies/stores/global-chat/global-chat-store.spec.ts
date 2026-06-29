import { Subject } from 'rxjs';
import {
  ChatMessage,
  ConnectionStatus,
  GlobalChatService,
} from '../../services/global-chat/global-chat-service';
import { TestBed } from '@angular/core/testing';
import { GlobalChatStore } from './global-chat-store';

describe('GlobalChatStore', () => {
  let globalChatService: {
    message$: Subject<ChatMessage>;
    connectionStatus$: Subject<ConnectionStatus>;

    connectCalled: boolean;
    connect(): void;

    disconnectCalled: boolean;
    disconnect(): void;

    lastMessageSent: string;
    sendMessage(message: string): void;
  };

  beforeEach(() => {
    globalChatService = {
      message$: new Subject<ChatMessage>(),
      connectionStatus$: new Subject<ConnectionStatus>(),

      connectCalled: false,
      connect(): void {
        this.connectCalled = true;
      },

      disconnectCalled: false,
      disconnect(): void {
        this.disconnectCalled = true;
      },

      lastMessageSent: '',
      sendMessage(message: string): void {
        this.lastMessageSent = message;
      },
    };

    TestBed.configureTestingModule({
      providers: [{ provide: GlobalChatService, useValue: globalChatService }],
    });
  });

  test.for<ConnectionStatus>(['Connected', 'Connecting', 'Disconnected'])(
    'sets status when connectionStatus$ emits %s',
    (status: ConnectionStatus) => {
      // given
      const store = TestBed.inject(GlobalChatStore);

      // when
      globalChatService.connectionStatus$.next(status);

      // then
      expect(store.status()).toEqual(status);
    },
  );

  test.for<ChatMessage>([
    {
      userId: 'user-1',
      username: 'alice',
      content: 'hello world',
      time: new Date('2024-01-15T09:00:00.000Z'),
    },
    {
      userId: 'user-2',
      username: 'bob',
      content: 'this is the message',
      time: new Date('2024-03-22T14:30:00.000Z'),
    },
    {
      userId: 'user-3',
      username: 'charlie',
      content: '',
      time: new Date('2024-06-01T00:00:00.000Z'),
    },
    {
      userId: 'user-4',
      username: 'dave',
      content: 'a'.repeat(500),
      time: new Date('2024-12-31T23:59:59.999Z'),
    },
    {
      userId: '',
      username: '',
      content: 'message with empty user fields',
      time: new Date('2024-07-04T12:00:00.000Z'),
    },
  ])('adds message to messages when message$ emits %s', (message: ChatMessage) => {
    // given
    const store = TestBed.inject(GlobalChatStore);

    // when
    globalChatService.message$.next(message);

    // then
    expect(store.messages()[0]).toEqual(message);
  });

  test('expand sets isExpanded to true', () => {
    // given
    const store = TestBed.inject(GlobalChatStore);

    // when
    store.expand();

    // then
    expect(store.isExpanded()).toBeTruthy();
  });

  test('collapse sets isExpanded to false', () => {
    // given
    const store = TestBed.inject(GlobalChatStore);

    // when
    store.collapse();

    // then
    expect(store.isExpanded()).toBeFalsy();
  });

  test('connect calls chatService.connect', async () => {
    // given
    const store = TestBed.inject(GlobalChatStore);

    // when
    await store.connect();

    // then
    expect(globalChatService.connectCalled).toBeTruthy();
  });

  test('disconnect calls chatService.disconnect', async () => {
    // given
    const store = TestBed.inject(GlobalChatStore);

    // when
    await store.disconnect();

    // then
    expect(globalChatService.disconnectCalled).toBeTruthy();
  });

  test.for<string>(['', 'a', 'testing', 'this is a longer message112!!§§'])(
    'sendMessage calls chatService.sendMessage with %s',
    async (message: string) => {
      // given
      const store = TestBed.inject(GlobalChatStore);

      // when
      await store.sendMessage(message);

      // then
      expect(globalChatService.lastMessageSent).toEqual(message);
    },
  );
});
